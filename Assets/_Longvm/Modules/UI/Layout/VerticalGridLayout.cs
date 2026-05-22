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
    [SerializeField] bool setContentHeight = true;
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
            if (setContentHeight)
            {
                content.SetSizeDeltaY(paddingTop + paddingBottom);
            }

            return;
        }

        itemsPerRow = Mathf.Max(1, itemsPerRow);

        int rowCount = Mathf.CeilToInt((float)totalItemCount / itemsPerRow);

        float totalHeight = CalculateTotalHeight(activeChildren, rowCount);

        if (setContentHeight)
        {
            content.SetSizeDeltaY(totalHeight);
        }

        float contentWidth = content.rect.width - paddingLeft - paddingRight;

        // Pivot center
        float currentY = totalHeight * 0.5f - paddingTop;

        for (int row = 0; row < rowCount; row++)
        {
            int startIndex = row * itemsPerRow;

            int itemCountInRow = Mathf.Min(
                itemsPerRow,
                totalItemCount - startIndex
            );

            bool isLastRow = row == rowCount - 1;
            bool isIncompleteRow = itemCountInRow < itemsPerRow;

            float rowHeight = 0f;
            float rowWidth = 0f;

            // Calculate row size
            for (int i = 0; i < itemCountInRow; i++)
            {
                RectTransform child = activeChildren[startIndex + i];

                rowHeight = Mathf.Max(rowHeight, child.rect.height);
                rowWidth += child.rect.width;
            }

            if (itemCountInRow > 1)
            {
                rowWidth += horizontalSpacing * (itemCountInRow - 1);
            }

            float startX;

            bool shouldAlignLeft =
                isLastRow &&
                isIncompleteRow &&
                !centerLastIncompleteRow;

            if (shouldAlignLeft)
            {
                float fullRowWidth = 0f;

                for (int i = 0; i < itemsPerRow; i++)
                {
                    RectTransform refChild = activeChildren[i];

                    fullRowWidth += refChild.rect.width;
                }

                if (itemsPerRow > 1)
                {
                    fullRowWidth += horizontalSpacing * (itemsPerRow - 1);
                }

                startX =
                    -contentWidth * 0.5f +
                    (contentWidth - fullRowWidth) * 0.5f;
            }
            else
            {
                startX =
                    -contentWidth * 0.5f +
                    (contentWidth - rowWidth) * 0.5f;
            }

            float currentX = startX;

            for (int i = 0; i < itemCountInRow; i++)
            {
                RectTransform child = activeChildren[startIndex + i];

                float w = child.rect.width;
                float h = child.rect.height;

                float posX = currentX + w * 0.5f;
                float posY = currentY - h * 0.5f;

                child.anchoredPosition = new Vector2(posX, posY);

                currentX += w + horizontalSpacing;
            }

            currentY -= rowHeight + verticalSpacing;
        }
    }

    float CalculateTotalHeight(
        List<RectTransform> activeChildren,
        int rowCount
    )
    {
        float totalHeight = paddingTop + paddingBottom;

        for (int row = 0; row < rowCount; row++)
        {
            int startIndex = row * itemsPerRow;

            int itemCountInRow = Mathf.Min(
                itemsPerRow,
                activeChildren.Count - startIndex
            );

            float rowHeight = 0f;

            for (int i = 0; i < itemCountInRow; i++)
            {
                rowHeight = Mathf.Max(
                    rowHeight,
                    activeChildren[startIndex + i].rect.height
                );
            }

            totalHeight += rowHeight;
        }

        if (rowCount > 1)
        {
            totalHeight += verticalSpacing * (rowCount - 1);
        }

        return totalHeight;
    }
}
