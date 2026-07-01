using TMPro;
using Toolkit;
using UnityEngine;

public class PopupLose : Popup
{
    [SerializeField] UIAnimator openRevive;
    [SerializeField] UIAnimator closeRevive;
    [SerializeField] UIAnimator openLose;
    [SerializeField] TMP_Text revivePriceText;
    [SerializeField] TMP_Text reviveSecondsText;

    public override void Open()
    {
        base.Open();

        revivePriceText.text = GameConf.Ins.RevivePrice.ToString();
        reviveSecondsText.text = $"+{GameConf.Ins.ReviveSeconds}s";

        openRevive.gameObject.SetActive(true);
        openRevive.InitAndPlay();
        openLose.gameObject.SetActive(false);
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

    public void OnClickGiveUp()
    {
        closeRevive.Play();
        openLose.gameObject.SetActive(true);
        openLose.InitAndPlay();
        LevelManager.Ins.LoseLevel();
    }

    public void OnClickRetry()
    {
        SceneController.Ins.ToGame();
    }

    public void OnClickBackHome()
    {
        if (GamePref.Ins.LevelIndex + 1 < GameConf.Ins.BackHomeLevel)
        {
            SceneController.Ins.ToGame();
        }
        else
        {
            SceneController.Ins.ToHome();
        }
    }
    #endregion
}
