using UnityEngine;

/// <summary>
/// MonoBehaviour that drains queued main-thread events each Update().
/// Automatically created by EventBus on first use.
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    void Update()
    {
        EventBus.DrainMainThreadQueue();
    }

    void OnDestroy()
    {
        // When the dispatcher dies (domain reload / play stop), clear queued actions to avoid leaks
        EventBus.ClearAll();
    }
}
