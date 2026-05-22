using DG.Tweening;
using System;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public class MoveAnchorPosition : Animation
    {
        [SerializeField] Vector2 start;
        [SerializeField] Vector2 end;
        [SerializeField] Ease ease = Ease.OutQuad;
        [SerializeField] RectTransform rt;

        Tween tween;

        public override void Init()
        {
            if (tween == null || !tween.IsActive())
            {
                rt.anchoredPosition = start;
            }
            else
            {
                tween.Goto(0);
            }
        }

        public override void Play()
        {
            if (tween == null || !tween.IsActive())
            {
                tween = rt.DOAnchorPos(end, duration).SetEase(ease).SetAutoKill(false);
            }
            else
            {
                tween.Restart();
            }
        }
    }
}
