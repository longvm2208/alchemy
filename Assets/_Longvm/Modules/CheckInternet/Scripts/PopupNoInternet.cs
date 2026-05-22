using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupNoInternet : Popup
{
    [SerializeField] GameObject okTextObj;
    [SerializeField] GameObject loadingObj;
    [SerializeField] Button okButton;

    bool waitingForInternet;

    public override void Open()
    {
        base.Open();
        okButton.interactable = true;
        loadingObj.SetActive(false);
        okTextObj.SetActive(true);
    }

    public void WaitForInternet(bool wait)
    {
        waitingForInternet = wait;
    }

    IEnumerator WaitInternetCoroutine()
    {
        yield return new WaitUntil(() => CheckInternetManager.Ins.IsInternetAvailable());
        okTextObj.SetActive(true);
        loadingObj.SetActive(false);
        Close();
    }

    #region UI Events
    public void OnClickOk()
    {
        if (waitingForInternet)
        {
            okTextObj.SetActive(false);
            loadingObj.SetActive(true);
            okButton.interactable = false;
            StartCoroutine(WaitInternetCoroutine());
        }
        else
        {
            Close();
        }
    }
    #endregion
}
