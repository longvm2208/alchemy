using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class VerticalLayout : MonoBehaviour
{
    [SerializeField] bool reverseArrangement;
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;
    [SerializeField] float spacing;
    public float Spacing => spacing;
    [SerializeField] bool setContentHeight = true;
    [SerializeField] RectTransform content;

    [Button]
    public void UpdateLayout()
    {
        List<RectTransform> activeChilds = new();

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i) as RectTransform;

            if (child.gameObject.activeSelf)
            {
                activeChilds.Add(child);
            }
        }

        if (reverseArrangement)
        {
            activeChilds.Reverse();
        }

        int count = activeChilds.Count;

        float contentHeight = 0f;
        for (int i = 0; i < count; i++)
        {
            contentHeight += activeChilds[i].rect.height;
        }

        if (count > 1)
        {
            contentHeight += spacing * (count - 1);
        }

        float totalHeight = contentHeight + paddingTop + paddingBottom;

        if (setContentHeight)
        {
            content.SetSizeDeltaY(totalHeight);
        }

        float currentY = totalHeight * 0.5f - paddingTop;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = activeChilds[i];
            float h = child.rect.height;
            float posY = currentY - h * 0.5f;
            child.SetAnchorPosY(posY);
            currentY -= h + spacing;
        }
    }
}
