using System.Collections.Generic;
using UnityEngine;

public class CombinationGroup : MonoBehaviour
{
    [SerializeField] CombinationView[] combinationViews;

    public void Init(List<MergeRecipe> recipes)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            combinationViews[i].gameObject.SetActive(true);
            combinationViews[i].Init(recipes[i]);
        }

        if (recipes.Count < combinationViews.Length)
        {
            for (int i = recipes.Count; i < combinationViews.Length; i++)
            {
                combinationViews[i].gameObject.SetActive(false);
            }
        }
    }
}
