using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Toolkit
{
    public abstract class RewardAnimation : MonoBehaviour
    {
        [SerializeField] protected RectTransform myRt;
        public RectTransform MyRt => myRt;
        [SerializeField] protected TMP_Text amountText;
        [SerializeReference] protected AmountFormatter formatter;
        [SerializeField] UnityEvent onPlay;

        protected Action onReceiveReward;
        protected Action onComplete;

        private void OnValidate()
        {
            if (myRt == null) myRt = transform as RectTransform;
        }

        public abstract void SetDestination(Transform destination);

        public virtual void Init(int amount) => amountText.text = formatter.Format(amount);

        public void OnReceiveReward(Action onReceiveReward) => this.onReceiveReward = onReceiveReward;

        public void OnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
        }

        public virtual void Play()
        {
            onPlay?.Invoke();
        }
    }
}
