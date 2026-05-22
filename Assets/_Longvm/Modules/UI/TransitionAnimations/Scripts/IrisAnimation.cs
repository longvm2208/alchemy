using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class IrisAnimation : TransitionAnimationBase
{
    [SerializeField] float closeLength;
    [SerializeField] float openLength;
    [SerializeField] float openScale;
    [SerializeField] RectTransform maskTransform;

    public override Coroutine Close(Action onComplete = null)
    {
        gameObject.SetActive(true);
        return StartCoroutine(CloseCoroutine(onComplete));
    }

    IEnumerator CloseCoroutine(Action onComplete)
    {
        if (maskTransform == null) yield break;
        //AudioManager.Ins.PlaySound(SoundType.sfx_game_transfer);
        maskTransform.DOKill();
        maskTransform.localScale = openScale * Vector3.one;
        maskTransform.DOScale(0, closeLength).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(closeLength);
        onComplete?.Invoke();
    }

    public override Coroutine Open(Action onComplete = null)
    {
        gameObject.SetActive(true);
        return StartCoroutine(OpenCoroutine(onComplete));
    }

    IEnumerator OpenCoroutine(Action onComplete)
    {
        if (maskTransform == null) yield break;
        //AudioManager.Ins.PlaySound(SoundType.sfx_game_transfer);
        maskTransform.DOKill();
        maskTransform.localScale = Vector3.zero;
        maskTransform.DOScale(openScale * Vector3.one, openLength).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(openLength);
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
