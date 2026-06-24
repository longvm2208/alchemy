using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtraTimeButton : ExtendedMonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject lockedObj;
    [SerializeField] TMP_Text unlockLevelText;
    [SerializeField] GameObject unlockedObj;
    [SerializeField] GameObject countObj;
    [SerializeField] TMP_Text countText;
    [SerializeField] GameObject priceObj;
    [SerializeField] TMP_Text priceText;
    [SerializeField] RectTransform iconRect;
    public RectTransform IconRect => iconRect;
    [SerializeField] ParticleSystem particle;

    private void Awake()
    {
        EventBus.Subscribe<ChangeExtraTimeCount>(OnExtraTimeCountChanged);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeExtraTimeCount>(OnExtraTimeCountChanged);
    }

    public void Init()
    {
        int currentLevel = GamePref.Ins.LevelIndex + 1;
        bool isUnlocked = currentLevel >= GameConf.Ins.ExtraTimeLevel;

        button.interactable = isUnlocked;
        lockedObj.SetActive(!isUnlocked);
        unlockLevelText.text = "Level " + GameConf.Ins.ExtraTimeLevel.ToString();

        unlockedObj.SetActive(isUnlocked);
        UpdateView();
    }

    void UpdateView()
    {
        countObj.SetActive(GamePref.Ins.ExtraTimeCount > 0);
        countText.text = GamePref.Ins.ExtraTimeCount.ToString();

        priceObj.SetActive(GamePref.Ins.ExtraTimeCount == 0);
        priceText.text = $"{GameConf.Ins.ExtraTimePrice}";
    }

    public void PlayParticle()
    {
        if (particle != null)
        {
            particle.Play();
        }
        AudioManager.Ins.PlaySound(SoundType.sfx_game_claim_booster);
    }

    #region Event Listeners
    public void OnExtraTimeCountChanged(ChangeExtraTimeCount _)
    {
        UpdateView();
    }

    public void OnClick()
    {
        LevelManager.Ins.OnUserInteract();
        //GameTutorial.Ins.NextMagnetStep();

        string placement = "ingame";

        if (GamePref.Ins.ExtraTimeCount > 0)
        {
            if (BoosterManager.Ins.ExecuteExtraTime())
            {
                GamePref.Ins.RemoveExtraTime(1, placement, true);
            }
        }
        else if (GamePref.Ins.CoinCount >= GameConf.Ins.ExtraTimePrice)
        {
            if (BoosterManager.Ins.ExecuteExtraTime())
            {
                GamePref.Ins.RemoveCoin(GameConf.Ins.ExtraTimePrice, placement, "buy_extra_time", true);
                //FloatingResourceManager.Ins.Float(priceObj.transform.position, RenderMode.ScreenSpaceOverlay, new RewardConfig(RewardType.Coin, GameConf.Ins.MagnetPrice));
            }
        }
        else
        {
            //UIManager.Ins.Open<PopupShop>();
        }
    }
    #endregion
}
