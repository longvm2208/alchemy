using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit
{
    [Serializable]
    public class FadeGraphic : Animation
    {
        [SerializeField] float start;
        [SerializeField] float end;
        [SerializeField] Graphic graphic;

        Tween tween;

        public override void Init()
        {
            if (tween == null || !tween.IsActive())
            {
                graphic.SetAlpha(start);
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
                tween = graphic.DOFade(end, duration).SetAutoKill(true);
            }
            else
            {
                tween.Restart();
            }
        }
    } 
}
