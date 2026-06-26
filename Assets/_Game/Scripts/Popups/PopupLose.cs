using TMPro;
using UnityEngine;

public class PopupLose : Popup
{
    [SerializeField] TMP_Text revivePriceText;
    [SerializeField] TMP_Text reviveSecondsText;

    public override void Open()
    {
        base.Open();

        revivePriceText.text = GameConf.Ins.RevivePrice.ToString();
        reviveSecondsText.text = $"+{GameConf.Ins.ReviveSeconds}s";
    }

    #region Event Listeners
    public void OnClickReviveCoin()
    {
        if (GamePref.Ins.CoinCount >= GameConf.Ins.RevivePrice)
        {
            GamePref.Ins.RemoveCoin(GameConf.Ins.RevivePrice, "", "");
            LevelManager.Ins.ReviveLevel();
            Close();
        }
        else
        {
            // shop
        }
    }

    public void OnClickReviveAds()
    {
        // ads
        LevelManager.Ins.ReviveLevel();
        Close();
    }

    public void OnClickClose()
    {
        LevelManager.Ins.LoseLevel();
        Close();
    }
    #endregion
}
