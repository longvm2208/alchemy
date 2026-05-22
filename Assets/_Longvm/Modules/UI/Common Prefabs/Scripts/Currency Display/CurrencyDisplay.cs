using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyDisplay : MonoBehaviour
{
    [System.Serializable]
    public class CurrencyAmount
    {
        [SerializeField, Range(0f, 2f)] float duration = 1f;
        [SerializeField] TMP_Text amountText;

        int previousAmount;

        public void Init(int amount)
        {
            previousAmount = amount;
            SetText(amount);
        }

        void SetText(int amount)
        {
            amountText.text = amount.ToString();
        }

        public void Tween(int currentAmount)
        {
            DOVirtual.Int(previousAmount, currentAmount, duration, amount =>
            {
                SetText(amount);
            });

            previousAmount = currentAmount;
        }
    }

    [System.Serializable]
    public class CurrencyIcon
    {
        [SerializeField, Range(0f, 1f)] float duration = 1f;
        [SerializeField, Range(1f, 2f)] float scale = 1.2f;
        [SerializeField] Transform transform;
        [SerializeField] AnimationCurve curve;
        [SerializeField] UnityEvent onTween;

        Tween tween;

        public void Tween()
        {
            if (transform == null) return;

            if (tween == null)
            {
                tween = transform.DOScale(scale * transform.localScale, duration).SetEase(curve).SetAutoKill(false);
            }
            else
            {
                tween.Restart();
            }

            onTween?.Invoke();
        }
    }

    [SerializeField] CurrencyAmount amount;
    public CurrencyAmount Amount => amount;
    [SerializeField] CurrencyIcon icon;
    public CurrencyIcon Icon => icon;
}
