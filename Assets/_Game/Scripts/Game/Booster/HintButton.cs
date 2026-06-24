using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintButton : ExtendedMonoBehaviour
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
        EventBus.Subscribe<ChangeHintCount>(OnHintCountChanged);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeHintCount>(OnHintCountChanged);
    }

    public void Init()
    {
        int currentLevel = GamePref.Ins.LevelIndex + 1;
        bool isUnlocked = currentLevel >= GameConf.Ins.HintLevel;

        button.interactable = isUnlocked;
        lockedObj.SetActive(!isUnlocked);
        unlockLevelText.text = "Level " + GameConf.Ins.HintLevel.ToString();

        unlockedObj.SetActive(isUnlocked);
        UpdateView();
    }

    void UpdateView()
    {
        countObj.SetActive(GamePref.Ins.HintCount > 0);
        countText.text = GamePref.Ins.HintCount.ToString();

        priceObj.SetActive(GamePref.Ins.HintCount == 0);
        priceText.text = $"{GameConf.Ins.HintPrice}";
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
    public void OnHintCountChanged(ChangeHintCount _)
    {
        UpdateView();
    }

    public void OnClick()
    {
        LevelManager.Ins.OnUserInteract();
        //GameTutorial.Ins.NextMagnetStep();

        string placement = "ingame";

        if (GamePref.Ins.HintCount > 0)
        {
            if (BoosterManager.Ins.ExecuteHint())
            {
                GamePref.Ins.RemoveHint(1, placement, true);
            }
        }
        else if (GamePref.Ins.CoinCount >= GameConf.Ins.HintPrice)
        {
            if (BoosterManager.Ins.ExecuteHint())
            {
                GamePref.Ins.RemoveCoin(GameConf.Ins.HintPrice, placement, "buy_hint", true);
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
