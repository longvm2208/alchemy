using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;

public class MakeTable : MonoBehaviour
{
    [SerializeField] SimpleScrollSnap scrollSnap;
    [SerializeField] Transform disable;
    [SerializeField] Transform content;
    [SerializeField] MakeElement elementPrefab;

    List<MakeElement> elements;

    public void Init(List<ItemId> itemIds)
    {
        if (elements != null)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Destroy();
                elements[i].transform.SetParent(disable);
            }

            elements.Clear();
        }
        else
        {
            elements = new List<MakeElement>();
        }

        itemIds.Sort((a, b) =>
        {
            bool aDiscovered = GamePref.Ins.DiscoveredItems.Contains(a);
            bool bDiscovered = GamePref.Ins.DiscoveredItems.Contains(b);

            if (aDiscovered == bDiscovered)
            {
                return 0;
            }

            return aDiscovered ? -1 : 1;
        });

        for (int i = 0; i < itemIds.Count; i++)
        {
            MakeElement element = elementPrefab.GetInstance(content, true);
            element.transform.SetAsLastSibling();
            element.transform.localPosition = Vector3.zero;
            element.Init(itemIds[i]);
            elements.Add(element);
        }

        scrollSnap.Init();
    }
}
