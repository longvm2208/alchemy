using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] TMP_Text winRewardText;
    [SerializeField] ParticleImage coinParticle;
    [SerializeField] GameObject claimX2;

    public override void Open()
    {
        base.Open();

        coinParticle.onFirstParticleFinish.RemoveAllListeners();
        coinParticle.onLastParticleFinish.RemoveAllListeners();

        coinParticle.onFirstParticleFinish.AddListener(OnFirstParticle);
        coinParticle.onLastParticleFinish.AddListener(OnLastParticle);

        winRewardText.text = $"+{GameConf.Ins.WinReward}";

        claimX2.SetActive(GamePref.Ins.LevelIndex > 10);
    }

    void Claim(int x)
    {
        GamePref.Ins.AddCoin(x * GameConf.Ins.WinReward, "");
        coinParticle.Play();
        UIManager.Ins.EnableBlocker(true);
    }

    void OnFirstParticle()
    {
        EventBus.Publish(new ChangeCoinCount());
    }

    void OnLastParticle()
    {
        UIManager.Ins.EnableBlocker(false);
        SceneController.Ins.ToHome();
        Close();
    }

    #region Event Listeners
    public void OnClickClaim()
    {
        Claim(1);
    }

    public void OnClickClaimX2()
    {
        // ads
        Claim(2);
    }
    #endregion
}
