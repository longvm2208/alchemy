using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalLayout : MonoBehaviour
{
    [SerializeField] bool reverseArrangement;
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;
    [SerializeField] float spacing;
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

        float contentWidth = 0f;
        for (int i = 0; i < count; i++)
        {
            contentWidth += activeChilds[i].rect.width;
        }

        if (count > 1)
        {
            contentWidth += spacing * (count - 1);
        }

        float totalWidth = contentWidth + paddingLeft + paddingRight;
        content.SetSizeDeltaX(totalWidth);

        float currentX = -totalWidth * 0.5f + paddingLeft;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = activeChilds[i];
            float h = child.rect.width;
            float posX = currentX + h * 0.5f;
            child.SetAnchorPosX(posX);
            currentX += h + spacing;
        }
    }
}
