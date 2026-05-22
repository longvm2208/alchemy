using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinationView : PooledMonoBehaviour<CombinationView>
{
    [SerializeField] Image itemAImage;
    [SerializeField] Image itemBImage;
    [SerializeField] TMP_Text itemAText;
    [SerializeField] TMP_Text itemBText;

    public void Init(MergeRecipe recipe)
    {
        ItemDefinition itemADef = DatabaseManager.Ins.GetItemDefinition(recipe.ItemAId);
        ItemDefinition itemBDef = DatabaseManager.Ins.GetItemDefinition(recipe.ItemBId);
        itemAImage.sprite = itemADef.Icon;
        itemBImage.sprite = itemBDef.Icon;
        itemAText.text = itemADef.Name;
        itemBText.text = itemBDef.Name;
    }

    public void Destroy()
    {
        Recycle();
    }
}
