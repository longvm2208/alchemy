using UnityEngine;
using UnityEngine.UI;

public class CombinationView : MonoBehaviour
{
    [SerializeField] Sprite questionMark;
    [SerializeField] Image itemA;
    [SerializeField] Image itemB;

    public void Init(MergeRecipe recipe)
    {
        if (GamePref.Ins.DiscoveredRecipes.Contains(recipe.Id))
        {
            if (GamePref.Ins.DiscoveredItems.Contains(recipe.ItemAId))
            {
                ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(recipe.ItemAId);
                itemA.sprite = item.Icon;
            }
            else
            {
                itemA.sprite = questionMark;
            }

            if (GamePref.Ins.DiscoveredItems.Contains(recipe.ItemBId))
            {
                ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(recipe.ItemBId);
                itemB.sprite = item.Icon;
            }
            else
            {
                itemB.sprite = questionMark;
            }
        }
        else
        {
            itemA.sprite = questionMark;
            itemB.sprite = questionMark;
        }
    }
}
