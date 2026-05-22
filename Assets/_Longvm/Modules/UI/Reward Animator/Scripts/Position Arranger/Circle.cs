using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [Serializable]
    public class Circle : RewardArranger
    {
        [SerializeField] float duration;
        [SerializeField] Vector2 center;
        [SerializeField] float[] radiuses;

        public override void Init(List<RectTransform> rts)
        {
            base.Init(rts);
            if (rts.IsNullOrEmpty()) return;
            for (int i = 0; i < rts.Count; i++)
            {
                rts[i].localScale = Vector3.zero;
                rts[i].gameObject.SetActive(true);
            }
            if (rts.Count == 1)
            {
                rts[0].localPosition = center;
                return;
            }
            float angleModifier = 90f;
            if (rts.Count == 2) angleModifier = 0f;
            float radius = radiuses[rts.Count - 1];
            float angleStep = 360f / rts.Count;
            for (int i = 0; i < rts.Count; i++)
            {
                float angle = i * angleStep + angleModifier;
                float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                rts[i].localPosition = center + new Vector2(x, y);
            }
        }

        public override void Play()
        {
            for (int i = 0; i < rts.Count; i++)
            {
                if (i < rts.Count - 1) rts[i].DOScale(1f, duration).SetEase(Ease.OutBack);
                else rts[i].DOScale(1f, duration).SetEase(Ease.OutBack).OnComplete(() => isComplete = true);
            }
        }
    }
}
