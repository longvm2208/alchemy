using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementTable : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] PaginationButton previousButton;
    [SerializeField] PaginationButton nextButton;
    [SerializeField] TMP_Text currentTabText;
    [SerializeField] ElementTab[] elementTabs;

    int currentIndex;
    int tabCount;
    List<ItemDefinition> elements;

    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();

    bool canNext => currentIndex < tabCount - 1;
    bool canPrevious => currentIndex > 0;
    ElementTab previousTab => elementTabs[(currentIndex - 1 + elementTabs.Length) % elementTabs.Length];
    ElementTab currentTab => elementTabs[currentIndex % elementTabs.Length];
    ElementTab nextTab => elementTabs[(currentIndex + 1) % elementTabs.Length];

    private void Awake()
    {
        previousButton.OnClick(OnClickPrevious);
        nextButton.OnClick(OnClickNext);
    }

    public void Init(CategoryDefinition category)
    {
        elements ??= new List<ItemDefinition>();
        elements.Clear();

        for (int i = 0; i < category.Items.Length; i++)
        {
            ItemId itemId = category.Items[i];
            ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(itemId);
            elements.Add(item);
        }

        currentIndex = 0;

        currentTab.Init(GetTabElements(currentIndex));

        for (int i = 0; i < elementTabs.Length; i++)
        {
            if (i == currentIndex)
            {
                elementTabs[i].Show();
            }
            else
            {
                elementTabs[i].Hide();
            }
        }

        tabCount = elements.Count / 24;
        if (elements.Count % 24 > 0)
        {
            tabCount++;
        }

        RefreshPagination();
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    public void Appear()
    {
        rect.SetAnchorPosX(300);
        rect.DOAnchorPosX(0, 0.35f);
        canvasGroup.DOFade(1, 0.35f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void Disappear()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        rect.DOAnchorPosX(300, 0.35f);
        canvasGroup.DOFade(0, 0.35f);
    }

    List<ItemDefinition> GetTabElements(int tabIndex)
    {
        List<ItemDefinition> tabElements = new();

        for (int i = tabIndex * 24; i < tabIndex * 24 + 24; i++)
        {
            if (i >= elements.Count) break;

            tabElements.Add(elements[i]);
        }

        return tabElements;
    }

    void RefreshPagination()
    {
        currentTabText.text = $"{currentIndex + 1}/{tabCount}";
        previousButton.SetEnable(canPrevious);
        nextButton.SetEnable(canNext);
    }
    
    void OnClickPrevious()
    {
        previousTab.Init(GetTabElements(currentIndex - 1));
        currentTab.Disappear();
        previousTab.Appear();
        currentIndex--;
        RefreshPagination();
    }

    void OnClickNext()
    {
        nextTab.Init(GetTabElements(currentIndex + 1));
        currentTab.Disappear();
        nextTab.Appear();
        currentIndex++;
        RefreshPagination();
    }
}
