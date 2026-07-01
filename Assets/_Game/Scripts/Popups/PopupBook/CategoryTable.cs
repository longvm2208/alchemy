using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTable : MonoBehaviour
{
    [SerializeField] CategoryTab[] categoryTabs;
    [SerializeField] PaginationButton previousButton;
    [SerializeField] PaginationButton nextButton;
    [SerializeField] TMP_Text currentTabText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] PopupBook book;

    int currentIndex = -1;
    int tabCount;

    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();

    bool canNext => currentIndex < tabCount - 1;
    bool canPrevious => currentIndex > 0;
    CategoryTab previousTab => categoryTabs[(currentIndex - 1 + categoryTabs.Length) % categoryTabs.Length];
    CategoryTab currentTab => categoryTabs[currentIndex % categoryTabs.Length];
    CategoryTab nextTab => categoryTabs[(currentIndex + 1) % categoryTabs.Length];
    CategoryDefinition[] categories => DatabaseManager.Ins.Categories;

    private void Awake()
    {
        previousButton.OnClick(OnClickPrevious);
        nextButton.OnClick(OnClickNext);
    }

    public void Init()
    {
        if (currentIndex < 0)
        {
            currentIndex = 0;

            currentTab.Init(GetTabCategories(currentIndex));

            for (int i = 0; i < categoryTabs.Length; i++)
            {
                if (i == currentIndex)
                {
                    categoryTabs[i].Show();
                }
                else
                {
                    categoryTabs[i].Hide();
                }
            }

            tabCount = categories.Length / 5;
            if (categories.Length % 5 > 0)
            {
                tabCount++;
            }

            RefreshPagination();
        }
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
        rect.SetAnchorPosX(-300);
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
        rect.DOAnchorPosX(-300, 0.35f);
        canvasGroup.DOFade(0, 0.35f);
    }

    public void SelectCategory(CategoryDefinition category)
    {
        book.SelectCategory(category);
    }

    List<CategoryDefinition> GetTabCategories(int tabIndex)
    {
        List<CategoryDefinition> tabCategories = new();

        for (int i = tabIndex * 5; i < tabIndex * 5 + 5; i++)
        {
            if (i >= categories.Length) break;

            tabCategories.Add(categories[i]);
        }

        return tabCategories;
    }

    void RefreshPagination()
    {
        currentTabText.text = $"{currentIndex + 1}/{tabCount}";
        previousButton.SetEnable(canPrevious);
        nextButton.SetEnable(canNext);
    }

    void OnClickPrevious()
    {
        previousTab.Init(GetTabCategories(currentIndex - 1));
        currentTab.Disappear(Side.Right);
        previousTab.Appear(Side.Left);
        currentIndex--;
        RefreshPagination();
    }

    void OnClickNext()
    {
        nextTab.Init(GetTabCategories(currentIndex + 1));
        currentTab.Disappear(Side.Left);
        nextTab.Appear(Side.Right);
        currentIndex++;
        RefreshPagination();
    }
}
