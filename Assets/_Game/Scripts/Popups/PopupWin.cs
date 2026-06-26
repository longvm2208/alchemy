using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] TMP_Text winRewardText;
    [SerializeField] ParticleImage coinParticle;

    public override void Open()
    {
        base.Open();

        coinParticle.onFirstParticleFinish.RemoveAllListeners();
        coinParticle.onLastParticleFinish.RemoveAllListeners();

        coinParticle.onFirstParticleFinish.AddListener(OnFirstParticle);
        coinParticle.onLastParticleFinish.AddListener(OnLastParticle);

        winRewardText.text = $"+{GameConf.Ins.WinReward}";
    }

    void Claim(int x)
    {
        GamePref.Ins.AddCoin(x * GameConf.Ins.WinReward, "");
    }

    void OnFirstParticle()
    {
        EventBus.Publish(new ChangeCoinCount());
    }

    void OnLastParticle()
    {
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
