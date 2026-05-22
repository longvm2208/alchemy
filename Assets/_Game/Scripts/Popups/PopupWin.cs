public class PopupWin : Popup
{
    #region Event Listeners
    public void OnClickNext()
    {
        SceneController.Ins.ToHome();
    }
    #endregion
}
