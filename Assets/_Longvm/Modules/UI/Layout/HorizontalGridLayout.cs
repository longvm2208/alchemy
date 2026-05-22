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
    [SerializeField] RectTransform content;

    [Button]
    public void UpdateLayout()
    {
        List<RectTransform> activeChildren = new();

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i) as RectTransform;

            if (child.gameObject.activeSelf)
            {
                activeChildren.Add(child);
            }
        }

        if (reverseArrangement)
        {
            activeChildren.Reverse();
        }

        int totalItemCount = activeChildren.Count;

        if (totalItemCount == 0)
        {
            if (setContentWidth)
            {
                content.SetSizeDeltaX(paddingLeft + paddingRight);
            }

            return;
        }

        itemsPerColumn = Mathf.Max(1, itemsPerColumn);

        int columnCount = Mathf.CeilToInt(
            (float)totalItemCount / itemsPerColumn
        );

        float totalWidth = CalculateTotalWidth(
            activeChildren,
            columnCount
        );

        if (setContentWidth)
        {
            content.SetSizeDeltaX(totalWidth);
        }

        float contentHeight =
            content.rect.height - paddingTop - paddingBottom;

        // Pivot center
        float currentX = -totalWidth * 0.5f + paddingLeft;

        for (int column = 0; column < columnCount; column++)
        {
            int startIndex = column * itemsPerColumn;

            int itemCountInColumn = Mathf.Min(
                itemsPerColumn,
                totalItemCount - startIndex
            );

            bool isLastColumn = column == columnCount - 1;
            bool isIncompleteColumn =
                itemCountInColumn < itemsPerColumn;

            float columnWidth = 0f;
            float columnHeight = 0f;

            // Calculate column size
            for (int i = 0; i < itemCountInColumn; i++)
            {
                RectTransform child = activeChildren[startIndex + i];

                columnWidth = Mathf.Max(columnWidth, child.rect.width);
                columnHeight += child.rect.height;
            }

            if (itemCountInColumn > 1)
            {
                columnHeight +=
                    verticalSpacing * (itemCountInColumn - 1);
            }

            float startY;

            bool shouldAlignTop =
                isLastColumn &&
                isIncompleteColumn &&
                !centerLastIncompleteColumn;

            if (shouldAlignTop)
            {
                float fullColumnHeight = 0f;

                for (int i = 0; i < itemsPerColumn; i++)
                {
                    RectTransform refChild = activeChildren[i];

                    fullColumnHeight += refChild.rect.height;
                }

                if (itemsPerColumn > 1)
                {
                    fullColumnHeight +=
                        verticalSpacing * (itemsPerColumn - 1);
                }

                startY =
                    contentHeight * 0.5f -
                    (contentHeight - fullColumnHeight) * 0.5f;
            }
            else
            {
                // Center column
                startY =
                    columnHeight * 0.5f +
                    (contentHeight - columnHeight) * 0.5f;
            }

            float currentY = startY;

            for (int i = 0; i < itemCountInColumn; i++)
            {
                RectTransform child = activeChildren[startIndex + i];

                float w = child.rect.width;
                float h = child.rect.height;

                float posX = currentX + w * 0.5f;
                float posY = currentY - h * 0.5f;

                child.anchoredPosition = new Vector2(posX, posY);

                currentY -= h + verticalSpacing;
            }

            currentX += columnWidth + horizontalSpacing;
        }
    }

    float CalculateTotalWidth(
        List<RectTransform> activeChildren,
        int columnCount
    )
    {
        float totalWidth = paddingLeft + paddingRight;

        for (int column = 0; column < columnCount; column++)
        {
            int startIndex = column * itemsPerColumn;

            int itemCountInColumn = Mathf.Min(
                itemsPerColumn,
                activeChildren.Count - startIndex
            );

            float columnWidth = 0f;

            for (int i = 0; i < itemCountInColumn; i++)
            {
                columnWidth = Mathf.Max(
                    columnWidth,
                    activeChildren[startIndex + i].rect.width
                );
            }

            totalWidth += columnWidth;
        }

        if (columnCount > 1)
        {
            totalWidth += horizontalSpacing * (columnCount - 1);
        }

        return totalWidth;
    }
}