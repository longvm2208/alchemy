using DG.Tweening;
using System;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public class ScaleTransform : Animation
    {
        [SerializeField] Vector3 start;
        [SerializeField] Vector3 end;
        [SerializeField] Ease ease = Ease.OutBack;
        [SerializeField] Transform transform;

        Tween tween;

        public override void Init()
        {
            if (tween == null || !tween.IsActive())
            {
                transform.localScale = start;
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
                tween = transform.DOScale(end, duration).SetEase(ease).SetAutoKill(false);
            }
            else
            {
                tween.Restart();
            }
        }
    } 
}
