using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalGridLayout : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] int itemsPerColumn = 3;
    [SerializeField] bool reverseArrangement;
    [SerializeField] bool centerLastIncompleteColumn = true;

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
            if (setContentWidth)
            {
                content.SetSizeDeltaX(paddingLeft + paddingRight);
            }

            if (setContentHeight)
            {
                content.SetSizeDeltaY(paddingTop + paddingBottom);
            }

            return;
        }

        itemsPerColumn = Mathf.Max(1, itemsPerColumn);

        RectTransform sample = children[0];

        float itemWidth = sample.rect.width;
        float itemHeight = sample.rect.height;

        int columnCount = Mathf.CeilToInt(itemCount / (float)itemsPerColumn);

        float totalWidth = paddingLeft + paddingRight + columnCount * itemWidth + (columnCount - 1) * horizontalSpacing;

        if (setContentWidth)
        {
            content.SetSizeDeltaX(totalWidth);
        }

        float contentHeight = content.rect.height - paddingTop - paddingBottom;

        float fullColumnHeight = itemsPerColumn * itemHeight + (itemsPerColumn - 1) * verticalSpacing;

        float totalHeight = fullColumnHeight + paddingTop + paddingBottom;

        if (setContentHeight)
        {
            content.SetSizeDeltaY(totalHeight);
        }

        float startX = -totalWidth * 0.5f + paddingLeft + itemWidth * 0.5f;

        for (int column = 0; column < columnCount; column++)
        {
            int startIndex = column * itemsPerColumn;

            int itemCountInColumn = Mathf.Min(itemsPerColumn, itemCount - startIndex);

            bool isLastColumn = column == columnCount - 1;
            bool isIncompleteColumn = itemCountInColumn < itemsPerColumn;

            float columnHeight = itemCountInColumn * itemHeight + (itemCountInColumn - 1) * verticalSpacing;

            float startY;

            if (isLastColumn && isIncompleteColumn && !centerLastIncompleteColumn)
            {
                // Align top
                startY = contentHeight * 0.5f - (contentHeight - fullColumnHeight) * 0.5f;
            }
            else
            {
                // Center vertically
                startY = columnHeight * 0.5f + (contentHeight - columnHeight) * 0.5f;
            }

            float x = startX + column * (itemWidth + horizontalSpacing);

            for (int row = 0; row < itemCountInColumn; row++)
            {
                RectTransform child = children[startIndex + row];

                float y = startY - row * (itemHeight + verticalSpacing) - itemHeight * 0.5f;

                child.anchoredPosition = new Vector2(x, y);
            }
        }
    }
}