using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNewItem : Popup
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;

    event Action onClose;

    public PopupNewItem Init(ItemId itemId)
    {
        ItemDefinition itemDefinition = DatabaseManager.Ins.GetItemDefinition(itemId);

        if (itemDefinition == null) return this;

        iconImage.sprite = itemDefinition.Icon;
        nameText.text = itemDefinition.Name;
        descriptionText.text = itemDefinition.Description;

        return this;
    }

    public void OnClose(Action onClose)
    {
        this.onClose = onClose;
    }

    public override void Disable()
    {
        onClose?.Invoke();
        onClose = null;
        base.Disable();
    }

    #region Event Listeners
    public void OnClickClose()
    {
        Close();
    }
    #endregion
}
