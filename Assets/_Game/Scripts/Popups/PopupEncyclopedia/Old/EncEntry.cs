using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncEntry : EntryBase
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Color evenColor;
    [SerializeField] Color oddColor;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;

    int index;

    PopupEncyclopedia _popup;
    PopupEncyclopedia popup
    {
        get
        {
            if (_popup == null) _popup = UIManager.Ins.Get<PopupEncyclopedia>();
            return _popup;
        }
    }

    public override void Init(int index)
    {
        base.Init(index);

        this.index = index;

        backgroundImage.color = index % 2 == 0 ? evenColor : oddColor;

        ItemDefinition itemDefinition = popup.UnlockedItems[index];
        iconImage.sprite = itemDefinition.Icon;
        nameText.text = itemDefinition.Name;
    }

    #region Event Listeners
    public void OnClick()
    {
        popup.OpenItemInfo(index);
    }
    #endregion
}
