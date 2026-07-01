using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementView : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text tempText;
    [SerializeField] Button button;

    bool discovered;
    ItemDefinition element;
    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();

    private void Awake()
    {
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void Init(ItemDefinition element)
    {
        this.element = element;
        discovered = GamePref.Ins.DiscoveredItems.Contains(element.Id);
        icon.sprite = element.Icon;
        tempText.text = element.Name;
        tempText.gameObject.SetActive(element.Icon == null);
        icon.color = discovered ? Color.white : Color.black;
        icon.SetAlpha(discovered ? 1f : 0.5f);
        tempText.color = discovered ? Color.white : Color.black;
        tempText.SetAlpha(discovered ? 1f : 0.5f);
    }

    public void Show()
    {
        rect.localScale = Vector3.one;
        button.interactable = true;
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        button.interactable = false;
    }

    public void Appear(float delay)
    {
        rect.DOComplete();
        button.interactable = false;
        rect.localScale = Vector3.zero;
        rect.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetDelay(delay).OnComplete(() =>
        {
            button.interactable = true;
        });
    }

    public void Disappear(float delay)
    {
        rect.DOComplete();
        button.interactable = false;
        rect.localScale = Vector3.one;
        rect.DOScale(0, 0.2f).SetDelay(delay);
    }

    void OnClick()
    {
        if (discovered)
        {
            UIManager.Ins.Open<PopupElement>().Init(element);
        }
    }
}
