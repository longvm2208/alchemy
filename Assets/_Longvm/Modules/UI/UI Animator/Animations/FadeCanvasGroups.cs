using DG.Tweening;
using System;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public class FadeCanvasGroups : Animation
    {
        [SerializeField] float start;
        [SerializeField] float end;
        [SerializeField] CanvasGroup[] groups;

        Sequence sequence;

        public override void Init()
        {
            if (sequence == null || !sequence.IsActive())
            {
                for (int i = 0; i < groups.Length; i++)
                {
                    groups[i].alpha = start;
                }
            }
            else
            {
                sequence.Goto(0);
            }
        }

        public override void Play()
        {
            if (sequence == null || !sequence.IsActive())
            {
                sequence = DOTween.Sequence().SetAutoKill(false);
                for (int i = 0; i < groups.Length; i++)
                {
                    sequence.Join(groups[i].DOFade(end, duration));
                }
            }
            else
            {
                sequence.Restart();
            }
        }
    } 
}
