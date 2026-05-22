using System;

/// <summary>
/// Token returned by Subscribe() so the caller can dispose to unsubscribe.
/// </summary>
public sealed class Subscription : IDisposable
{
    private Action _disposeAction;
    private bool _disposed;

    internal Subscription(Action dispose)
    {
        _disposeAction = dispose;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _disposeAction?.Invoke();
        _disposeAction = null;
    }
}
