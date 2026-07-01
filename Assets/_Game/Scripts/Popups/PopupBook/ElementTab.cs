using System.Collections.Generic;
using UnityEngine;

public class ElementTab : MonoBehaviour
{
    [SerializeField] ElementView[] elementViews;

    public void Init(List<ItemDefinition> elements)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            elementViews[i].gameObject.SetActive(true);
            elementViews[i].Init(elements[i]);
        }

        if (elements.Count < elementViews.Length)
        {
            for (int i = elements.Count; i < elementViews.Length; i++)
            {
                elementViews[i].gameObject.SetActive(false);
            }
        }
    }

    public void Show()
    {
        for (int i = 0; i < elementViews.Length; i++)
        {
            elementViews[i].Show();
        }
    }

    public void Hide()
    {
        for (int i = 0; i < elementViews.Length; i++)
        {
            elementViews[i].Hide();
        }
    }

    public void Appear()
    {
        for (int i = 0; i < elementViews.Length; i++)
        {
            if (!elementViews[i].gameObject.activeSelf) break;

            elementViews[i].Appear(0.075f + 0.015f * i);
        }
    }

    public void Disappear()
    {
        for (int i = 0; i < elementViews.Length; i++)
        {
            if (!elementViews[i].gameObject.activeSelf) break;

            elementViews[i].Disappear(0.015f * i);
        }
    }
}
