using System.Collections.Generic;
using UnityEngine;

public class CategoryTab : MonoBehaviour
{
    [SerializeField] CategoryTable categoryTable;
    [SerializeField] CategoryView[] categoryViews;

    public void Init(List<CategoryDefinition> categories)
    {
        for (int i = 0; i < categories.Count; i++)
        {
            categoryViews[i].gameObject.SetActive(true);
            categoryViews[i].Init(categories[i]);
        }

        if (categories.Count < categoryViews.Length)
        {
            for (int i = categories.Count; i < categoryViews.Length; i++)
            {
                categoryViews[i].gameObject.SetActive(false);
            }
        }
    }

    public void Show()
    {
        for (int i = 0; i < categoryViews.Length; i++)
        {
            categoryViews[i].Show();
        }
    }

    public void Hide()
    {
        for (int i = 0; i < categoryViews.Length; i++)
        {
            categoryViews[i].Hide();
        }
    }

    public void Appear(Side fromSide)
    {
        for (int i = 0; i < categoryViews.Length; i++)
        {
            if (!categoryViews[i].gameObject.activeSelf) break;

            categoryViews[i].Appear(fromSide, 0.15f + 0.075f * i);
        }
    }

    public void Disappear(Side toSide)
    {
        for (int i = 0; i < categoryViews.Length; i++)
        {
            if (!categoryViews[i].gameObject.activeSelf) break;

            categoryViews[i].Disappear(toSide, 0.075f * i);
        }
    }

    public void SelectCategory(CategoryDefinition category)
    {
        categoryTable.SelectCategory(category);
    }
}
