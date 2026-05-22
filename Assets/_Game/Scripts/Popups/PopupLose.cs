public class PopupLose : Popup
{
    #region Event Listeners
    public void OnClickRevive()
    {
        LevelManager.Ins.ReviveLevel();
        Close();
    }
    public void OnClickClose()
    {
        LevelManager.Ins.LoseLevel();
        Close();
    }
    #endregion
}
