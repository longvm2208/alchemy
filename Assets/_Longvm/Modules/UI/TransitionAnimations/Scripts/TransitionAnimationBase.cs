using System;
using UnityEngine;

public abstract class TransitionAnimationBase : MonoBehaviour
{
    public abstract Coroutine Close(Action onComplete = null);
    public abstract Coroutine Open(Action onComplete = null);
}
