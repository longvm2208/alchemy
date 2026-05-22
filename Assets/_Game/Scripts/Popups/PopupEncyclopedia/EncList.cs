public class EncList : ListBase
{
    PopupEncyclopedia _popup;
    PopupEncyclopedia popup
    {
        get
        {
            if (_popup == null) _popup = UIManager.Ins.Get<PopupEncyclopedia>();
            return _popup;
        }
    }

    protected override int GetCount()
    {
        return popup.UnlockedItems.Count;
    }
}
