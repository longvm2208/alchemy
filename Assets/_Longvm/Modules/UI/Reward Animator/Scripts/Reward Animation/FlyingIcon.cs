using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Toolkit
{
    public class FlyingIcon : RewardAnimation
    {
        [SerializeField] Transform destination;
        [SerializeField] float scale = 0.5f;
        [SerializeField] float duration;
        [SerializeField] RectTransform iconTransform;
        [SerializeField] UnityEvent onFinish;

        public override void SetDestination(Transform destination) => this.destination = destination;

        public override void Init(int amount)
        {
            base.Init(amount);
            amountText.gameObject.SetActive(true);
            iconTransform.localPosition = Vector3.zero;
            iconTransform.localScale = Vector3.one;
        }

        public override void Play()
        {
            amountText.gameObject.SetActive(false);
            iconTransform.DOScale(scale, duration);
            iconTransform.DOMove(destination.position, duration).SetEase(Ease.InCubic).OnComplete(OnComplete);

            void OnComplete()
            {
                onReceiveReward?.Invoke();
                onComplete?.Invoke();
                onFinish?.Invoke();
            }
        }
    }
}
