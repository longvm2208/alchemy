using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.UI;

public class PopupTimeFreeze : Popup
{
    [SerializeField] RectTransform particleDes;
    [SerializeField] ParticleImage particle;
    [SerializeField] Button claimButton;

    public override void Open()
    {
        base.Open();

        particle.onParticleFinish.RemoveListener(OnParticleFinish);
        particle.onParticleFinish.AddListener(OnParticleFinish);

        particle.onLastParticleFinish.RemoveListener(OnLastParticleFinish);
        particle.onLastParticleFinish.AddListener(OnLastParticleFinish);

        //particleDes.position = BoosterManager.Ins.MagnetButton.IconRect.position;
        claimButton.interactable = true;
    }

    #region Event Listeners
    public void OnClickClaim()
    {
        particle.Play();
        closeAnimator.InitAndPlay();
        claimButton.interactable = false;
    }

    void OnParticleFinish()
    {
        //BoosterManager.Ins.MagnetButton.PlayParticle();
    }

    void OnLastParticleFinish()
    {
        Disable();
        //GameTutorial.Ins.NextMagnetStep();
    }
    #endregion
}