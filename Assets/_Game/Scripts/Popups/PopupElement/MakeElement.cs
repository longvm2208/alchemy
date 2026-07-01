using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeElement : PooledMonoBehaviour<MakeElement>
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject questionMark;

    public void Init(ItemId itemId)
    {
        bool discovered = GamePref.Ins.DiscoveredItems.Contains(itemId);
        icon.gameObject.SetActive(discovered);
        ItemDefinition item = DatabaseManager.Ins.GetItemDefinition(itemId);
        icon.sprite = item.Icon;
        nameText.text = item.Name;
        questionMark.SetActive(!discovered);
    }

    public void Destroy()
    {
        Recycle();
    }
}