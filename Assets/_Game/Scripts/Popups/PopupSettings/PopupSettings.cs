using UnityEngine;

public class PopupSettings : Popup
{
	[SerializeField] RectTransform popupRect;
	[SerializeField] GameObject policyObj;
	[SerializeField] GameObject restoreObj;

    public override void Open()
    {
        base.Open();

		//policyObj.SetActive(MaxManager.Ins.PrivacyOptionsRequired());
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
