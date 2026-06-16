using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMakeView : PooledMonoBehaviour<ItemMakeView>
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;

    public void Init(MergeRecipe recipe)
    {
        ItemDefinition itemDef = DatabaseManager.Ins.GetItemDefinition(recipe.ResultItemId);
        iconImage.sprite = itemDef.Icon;
        nameText.text = itemDef.Name;
    }

    public void Destroy()
    {
        Recycle();
    }
}
