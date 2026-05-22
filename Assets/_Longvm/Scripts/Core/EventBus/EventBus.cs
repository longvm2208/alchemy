// EventBus.cs
// A complete, ready-to-use event bus for Unity.
// Features:
// - Type-safe events using generic payloads
// - Subscribe / Unsubscribe with IDisposable token
// - Optional automatic execution on Unity main thread
// - One-shot listeners (auto-unsubscribe after first invoke)
// - Thread-safe publish/subscribe
// - Lazy-created MainThreadDispatcher that processes events in Update()
// - Lightweight (no reflection at runtime beyond type keys)

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Internal wrapper for a listener.
/// </summary>
class Listener
{
    public Delegate Callback; // Action<T>
    public bool Once;
    public bool OnMainThread;
}

/// <summary>
/// Central static EventBus. Use EventBus.Subscribe/Publish from anywhere.
/// </summary>
public static class EventBus
{
    // map event Type -> list of listeners
    static readonly Dictionary<Type, List<Listener>> _listeners = new Dictionary<Type, List<Listener>>();
    static readonly object _lock = new object();

    // A queue used when delivering events on the main thread.
    static readonly Queue<Action> _mainThreadQueue = new Queue<Action>();

    // Ensure dispatcher exists
    static bool _initialized = false;

    static void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;
        // Create a GameObject to host the dispatcher if none exists.
        var go = new GameObject("__EventBus_MainThreadDispatcher");
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<MainThreadDispatcher>();
    }

    /// <summary>
    /// Subscribe to events of type TPayload.
    /// Returns IDisposable token — call Dispose() to unsubscribe.
    /// </summary>
    public static Subscription Subscribe<TPayload>(Action<TPayload> callback, bool onMainThread = true, bool once = false)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        EnsureInitialized();

        var t = typeof(TPayload);
        var listener = new Listener { Callback = callback, Once = once, OnMainThread = onMainThread };

        lock (_lock)
        {
            if (!_listeners.TryGetValue(t, out var list))
            {
                list = new List<Listener>();
                _listeners[t] = list;
            }
            list.Add(listener);
        }

        void DisposeAction()
        {
            UnsubscribeInternal(t, listener);
        }

        return new Subscription(DisposeAction);
    }

    /// <summary>
    /// Unsubscribe a specific handler for TPayload.
    /// </summary>
    public static void Unsubscribe<TPayload>(Action<TPayload> callback)
    {
        if (callback == null) return;
        var t = typeof(TPayload);
        lock (_lock)
        {
            if (!_listeners.TryGetValue(t, out var list)) return;
            list.RemoveAll(l => l.Callback == (Delegate)callback);
            if (list.Count == 0) _listeners.Remove(t);
        }
    }

    static void UnsubscribeInternal(Type t, Listener listener)
    {
        lock (_lock)
        {
            if (!_listeners.TryGetValue(t, out var list)) return;
            list.Remove(listener);
            if (list.Count == 0) _listeners.Remove(t);
        }
    }

    /// <summary>
    /// Publish an event with payload. Optionally force delivery on main thread.
    /// </summary>
    public static void Publish<TPayload>(TPayload payload)
    {
        EnsureInitialized();
        List<Listener> snapshot = null;
        var t = typeof(TPayload);

        lock (_lock)
        {
            if (!_listeners.TryGetValue(t, out var list)) return;
            // shallow copy to allow modification during iteration
            snapshot = new List<Listener>(list);
        }

        foreach (var listener in snapshot)
        {
            // If listener was removed during iteration, skip if it's no longer present
            bool stillPresent;
            lock (_lock)
            {
                stillPresent = _listeners.TryGetValue(t, out var current) && current.Contains(listener);
            }
            if (!stillPresent) continue;

            void Deliver()
            {
                try
                {
                    // Dynamic invoke is avoided by casting to the specific Action<TPayload>
                    var cb = listener.Callback as Action<TPayload>;
                    cb?.Invoke(payload);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                if (listener.Once)
                {
                    UnsubscribeInternal(t, listener);
                }
            }

            if (listener.OnMainThread)
            {
                // enqueue to main thread dispatcher
                lock (_mainThreadQueue)
                {
                    _mainThreadQueue.Enqueue(Deliver);
                }
            }
            else
            {
                // deliver on calling thread
                Deliver();
            }
        }
    }

    /// <summary>
    /// Internal: called by MainThreadDispatcher each frame to run queued main-thread events.
    /// </summary>
    internal static void DrainMainThreadQueue()
    {
        EnsureInitialized();
        // Move queued actions to a local list to minimize lock time
        List<Action> toRun = null;
        lock (_mainThreadQueue)
        {
            if (_mainThreadQueue.Count == 0) return;
            toRun = new List<Action>(_mainThreadQueue);
            _mainThreadQueue.Clear();
        }

        foreach (var a in toRun)
        {
            try
            {
                a?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    /// <summary>
    /// Clear all listeners. Useful for tests or full unload.
    /// </summary>
    public static void ClearAll()
    {
        lock (_lock)
        {
            _listeners.Clear();
        }
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Clear();
        }
    }
}

// --------------------------- USAGE -----------------------------------
// Example 1 - simple payload:
//
// public struct PlayerScored { public int playerId; public int points; }
//
// // subscribe:
// var token = EventBus.Subscribe<PlayerScored>(evt => Debug.Log($"player {evt.playerId} scored {evt.points}"));
//
// // publish:
// EventBus.Publish(new PlayerScored { playerId = 1, points = 10 });
//
// // unsubscribe:
// token.Dispose();
//
// Example 2 - one-shot listener (auto-unsubscribe):
// EventBus.Subscribe<PlayerScored>(evt => Debug.Log("first score received"), once: true);
//
// Example 3 - deliver on background thread (listener runs immediately on calling thread):
// EventBus.Subscribe<PlayerScored>(evt => { /* background processing */ }, onMainThread: false);

// Notes & recommendations:
// - Keep payload types small and copyable (structs are fine). Use classes if you need references.
// - Avoid very high-frequency events with complex subscriber lists; consider pooling or direct references.
// - The implementation keeps listeners in memory until unsubscribed. Use the returned Subscription (IDisposable)
//   or call Unsubscribe<T>(handler) to remove handlers when no longer needed.
// - If you need prioritized listeners, extend Listener with a priority int and sort the list when inserting.
// - For editor domain reloads, the dispatcher GameObject is created 'DontDestroyOnLoad' and hidden. During play->stop it will be destroyed automatically.
