using UnityEngine;

public class PopupSettings : Popup
{
	[SerializeField] RectTransform popupRect;
	[SerializeField] GameObject policyObj;
	[SerializeField] GameObject restoreObj;
	[SerializeField] float iosHeight;

    public override void Open()
    {
        base.Open();

		//policyObj.SetActive(MaxManager.Ins.PrivacyOptionsRequired());

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

	public void OnClickPolicy()
	{
		//MaxManager.Ins.ShowCmpForExistingUser();
	}
	#endregion
}
