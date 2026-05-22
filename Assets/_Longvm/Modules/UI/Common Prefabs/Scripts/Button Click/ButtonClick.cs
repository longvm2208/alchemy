using DG.Tweening;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] bool playAnimation = true;
    [SerializeField] bool playSound = true;
    [SerializeField] bool vibrate = true;
    [SerializeField] RectTransform buttonTransform;
    [SerializeField, ExposedScriptableObject] ButtonClickConfig config;

    Tween tween;

    private void OnValidate()
    {
        if (buttonTransform == null)
        {
            buttonTransform = transform as RectTransform;
        }
    }

    private void OnDestroy()
    {
        if (tween != null && tween.active)
        {
            tween.Kill();
        }
    }

    public void OnClick()
    {
        if (playAnimation)
        {
            if (tween == null)
            {
                if (buttonTransform != null)
                {
                    tween = buttonTransform.DOScale(config.Scale * buttonTransform.localScale, config.Duration).SetEase(config.Curve).SetAutoKill(false);
                }
            }
            else
            {
                tween.Restart();
            }
        }

        if (playSound)
        {
            AudioManager.Ins?.PlaySound(SoundType.sfx_ui_button_click);
        }

        if (vibrate)
        {
            VibrationManager.Ins?.Vibrate();
        }
    }
}
