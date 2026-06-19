using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class VerticalGridLayout : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] int itemsPerRow = 3;
    [SerializeField] bool reverseArrangement;
    [SerializeField] bool centerLastIncompleteRow = true;

    [Header("Spacing")]
    [SerializeField] float horizontalSpacing = 20f;
    [SerializeField] float verticalSpacing = 20f;

    [Header("Padding")]
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;

    [Header("Content")]
    [SerializeField] bool setContentWidth = true;
    [SerializeField] bool setContentHeight = true;
    [SerializeField] RectTransform content;

    [Button]
    public void UpdateLayout()
    {
        List<RectTransform> children = new();

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = (RectTransform)content.GetChild(i);

            if (child.gameObject.activeSelf)
            {
                children.Add(child);
            }
        }

        if (reverseArrangement)
        {
            children.Reverse();
        }

        int itemCount = children.Count;

        if (itemCount == 0)
        {
            Vector2 size = content.sizeDelta;

            if (setContentWidth)
            {
                size.x = paddingLeft + paddingRight;
            }

            if (setContentHeight)
            {
                size.y = paddingTop + paddingBottom;
            }

            content.sizeDelta = size;

            return;
        }

        itemsPerRow = Mathf.Max(1, itemsPerRow);

        RectTransform sample = children[0];

        float itemWidth = sample.rect.width;
        float itemHeight = sample.rect.height;

        int rowCount = Mathf.CeilToInt(itemCount / (float)itemsPerRow);

        float fullRowWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * horizontalSpacing;

        float totalWidth = fullRowWidth + paddingLeft + paddingRight;

        float totalHeight = rowCount * itemHeight + (rowCount - 1) * verticalSpacing + paddingTop + paddingBottom;

        Vector2 sizeDelta = content.sizeDelta;

        if (setContentWidth)
        {
            sizeDelta.x = totalWidth;
        }

        if (setContentHeight)
        {
            sizeDelta.y = totalHeight;
        }

        content.sizeDelta = sizeDelta;

        float contentWidth = content.rect.width - paddingLeft - paddingRight;

        float startY = totalHeight * 0.5f - paddingTop - itemHeight * 0.5f;

        for (int row = 0; row < rowCount; row++)
        {
            int startIndex = row * itemsPerRow;

            int itemCountInRow = Mathf.Min(itemsPerRow, itemCount - startIndex);

            bool isLastRow = row == rowCount - 1;
            bool isIncompleteRow = itemCountInRow < itemsPerRow;

            float rowWidth = itemCountInRow * itemWidth + (itemCountInRow - 1) * horizontalSpacing;

            float startX;

            if (isLastRow && isIncompleteRow && !centerLastIncompleteRow)
            {
                // Align left theo full row
                startX = -contentWidth * 0.5f + (contentWidth - fullRowWidth) * 0.5f + itemWidth * 0.5f;
            }
            else
            {
                // Center row
                startX = -contentWidth * 0.5f + (contentWidth - rowWidth) * 0.5f + itemWidth * 0.5f;
            }

            float y = startY - row * (itemHeight + verticalSpacing);

            for (int col = 0; col < itemCountInRow; col++)
            {
                RectTransform child = children[startIndex + col];

                float x = startX + col * (itemWidth + horizontalSpacing);

                child.anchoredPosition = new Vector2(x, y);
            }
        }
    }
}