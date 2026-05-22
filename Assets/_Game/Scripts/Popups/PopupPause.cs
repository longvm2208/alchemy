using UnityEngine;

public class PopupPause : Popup
{
    [SerializeField] GameObject homeObj;
    [SerializeField] RectTransform popupRect;
    [SerializeField] GameObject restoreObj;
    [SerializeField] float iosHeight;

    public override void Open()
    {
        base.Open();

        bool canBackHome = GamePref.Ins.LevelIndex > 0;
        homeObj.SetActive(canBackHome);

#if UNITY_IOS
		restoreObj.SetActive(true);
		popupRect.SetSizeDeltaY(iosHeight);		
#endif
    }

    #region Event Listeners
    public void OnClickClose()
    {
        Close();
    }

    public void OnClickLanguage()
    {
        //UIManager.Ins.Open<PopupLanguage>();
    }

    public void OnClickHome()
    {
        SceneController.Ins.ToHome();
    }
    #endregion
}
