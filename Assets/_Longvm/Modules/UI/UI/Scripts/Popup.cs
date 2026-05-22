using Sirenix.OdinInspector;
using Toolkit;
using UnityEngine;

public abstract class Popup : ExtendedMonoBehaviour
{
    [SerializeField] protected UIAnimator openAnimator;
    [SerializeField] protected UIAnimator closeAnimator;

    [Button]
    public virtual void Open()
    {
        UIManager.Ins.OnPopupOpen();
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        if (openAnimator)
        {
            AudioManager.Ins.PlaySound(SoundType.sfx_ui_popup_show);
            openAnimator.InitAndPlay();
        }
    }

    [Button]
    public virtual void Close()
    {
        if (closeAnimator)
        {
            AudioManager.Ins.PlaySound(SoundType.sfx_ui_popup_disappear);
            closeAnimator.Play(() =>
            {
                Disable();
            });
        }
        else
        {
            Disable();
        }
    }

    [Button]
    public virtual void Disable()
    {
        UIManager.Ins.OnPopupClose();
        gameObject.SetActive(false);
    }
}
