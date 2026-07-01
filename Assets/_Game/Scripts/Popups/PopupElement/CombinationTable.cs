using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinationTable : MonoBehaviour
{
    [SerializeField] SimpleScrollSnap scrollSnap;
    [SerializeField] Transform disable;
    [SerializeField] Transform content;
    [SerializeField] Transform[] scrollElements;
    [SerializeField] CombinationGroup[] groups;

    public void Init(List<MergeRecipe> recipes)
    {
        recipes.Sort((a, b) =>
        {
            bool aDiscovered = GamePref.Ins.DiscoveredRecipes.Contains(a.Id);
            bool bDiscovered = GamePref.Ins.DiscoveredRecipes.Contains(b.Id);

            if (aDiscovered == bDiscovered)
            {
                return 0;
            }

            return aDiscovered ? -1 : 1;
        });

        int groupCount = recipes.Count / 3;
        if (recipes.Count % 3 > 0)
        {
            groupCount++;
        }

        for (int i = 0; i < groupCount; i++)
        {
            scrollElements[i].SetParent(content);
            scrollElements[i].localPosition = Vector3.zero;
            scrollElements[i].SetAsLastSibling();

            List<MergeRecipe> groupRecipes = new();

            for (int j = i * 3; j < i * 3 + 3; j++)
            {
                if (j >= recipes.Count) break;

                groupRecipes.Add(recipes[j]);
            }

            groups[i].Init(groupRecipes);
        }

        if (groupCount < groups.Length)
        {
            for (int i = groupCount; i < groups.Length; i++)
            {
                scrollElements[i].SetParent(disable);
            }
        }

        scrollSnap.Init();
    }
}
