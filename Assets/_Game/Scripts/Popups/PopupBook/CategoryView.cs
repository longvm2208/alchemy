using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryView : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button button;
    [SerializeField] CategoryTab categoryTab;

    CategoryDefinition category;
    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();

    private void Awake()
    {
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void Init(CategoryDefinition category)
    {
        this.category = category;
        icon.sprite = category.Icon;
        icon.SetNativeSize();
        nameText.text = category.Name;
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Appear(Side fromSide, float delay)
    {
        rect.DOComplete();
        canvasGroup.DOComplete();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        float posX = fromSide == Side.Left ? -300 : 300;
        rect.SetAnchorPosX(posX);
        rect.DOAnchorPosX(0, 0.35f).SetDelay(delay);
        canvasGroup.DOFade(1, 0.35f).SetDelay(delay).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void Disappear(Side toSide, float delay)
    {
        rect.DOComplete();
        canvasGroup.DOComplete();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        float posX = toSide == Side.Left ? -300 : 300;
        rect.SetAnchorPosX(0);
        rect.DOAnchorPosX(posX, 0.35f).SetDelay(delay);
        canvasGroup.DOFade(0, 0.35f).SetDelay(delay);
    }

    void OnClick()
    {
        categoryTab.SelectCategory(category);
    }
}

public enum Side
{
    None,
    Left,
    Right,
}
