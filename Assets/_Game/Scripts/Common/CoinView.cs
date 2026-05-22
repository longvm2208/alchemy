using UnityEngine;

public class CoinView : MonoBehaviour
{
    [SerializeField] CurrencyDisplay display;

    private void OnEnable()
    {
        display.Amount.Init(GamePref.Ins.CoinCount);
        EventBus.Subscribe<ChangeCoinCount>(OnCoinCountChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ChangeCoinCount>(OnCoinCountChanged);
    }

    void OnCoinCountChanged(ChangeCoinCount _)
    {
        display.Amount.Tween(GamePref.Ins.CoinCount);
    }

    public void TweenIcon()
    {
        display.Icon.Tween();

        AudioManager.Ins.PlaySound(SoundType.sfx_ui_coin);
    }
}
