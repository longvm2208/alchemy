using System;
using System.Collections;
using UnityEngine;

public class FadeAnimation : TransitionAnimationBase
{
    [SerializeField] float closeLength;
    [SerializeField] float openLength;
    [SerializeField] Animator animator;

    readonly int openAnim = Animator.StringToHash("OpenAnim");
    readonly int closeAnim = Animator.StringToHash("CloseAnim");

    private void OnValidate()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public override Coroutine Close(Action onComplete = null)
    {
        gameObject.SetActive(true);
        return StartCoroutine(CloseCoroutine(onComplete));
    }

    IEnumerator CloseCoroutine(Action onComplete)
    {
        if (animator == null) yield break;
        animator.Play(closeAnim);
        yield return WaitFor.Seconds(closeLength);
        onComplete?.Invoke();
    }

    public override Coroutine Open(Action onComplete = null)
    {
        gameObject.SetActive(true);
        return StartCoroutine(OpenCoroutine(onComplete));
    }

    IEnumerator OpenCoroutine(Action onComplete = null)
    {
        if (animator == null) yield break;
        animator.Play(openAnim);
        yield return WaitFor.Seconds(openLength);
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
