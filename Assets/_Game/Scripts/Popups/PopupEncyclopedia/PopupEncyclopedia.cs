using System.Collections.Generic;
using Toolkit;
using UnityEngine;

public class PopupEncyclopedia : Popup
{
    [SerializeField] UIAnimator openItemInfo;
    [SerializeField] UIAnimator closeItemInfo;
    [SerializeField] EncList encList;
    [SerializeField] ItemInfo itemInfo;

    bool isItemInfo;

    List<ItemDefinition> unlockedItems;
    public List<ItemDefinition> UnlockedItems => unlockedItems;

    public override void Open()
    {
        base.Open();

        unlockedItems = DatabaseManager.Ins.GetUnlockedItems();

        encList.Init();

        isItemInfo = false;
        itemInfo.gameObject.SetActive(false);
    }

    public void OpenItemInfo(int index)
    {
        isItemInfo = true;
        itemInfo.gameObject.SetActive(true);
        itemInfo.Init(unlockedItems[index]);
        openItemInfo.Play();
    }

    #region Event Listeners
    public void OnClickClose()
    {
        if (isItemInfo)
        {
            closeItemInfo.Play(() =>
            {
                isItemInfo = false;
                itemInfo.gameObject.SetActive(false);
            });
        }
        else
        {
            Close();
        }
    }
    #endregion
}
