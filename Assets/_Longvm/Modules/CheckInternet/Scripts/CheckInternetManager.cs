using System;
using System.Collections;
using UnityEngine;

public class CheckInternetManager : SingletonMonoBehaviour<CheckInternetManager>
{
#if UNITY_EDITOR
    [SerializeField] bool isInternetAvailableOnEditor = true;
#endif
    [SerializeField] bool isContinuousCheck;
    [SerializeField] float checkInterval = 1f;

    public event Action OnNetworkStatusChanged;

    bool status;

    public void Init()
    {
        if (isContinuousCheck)
        {
            status = IsInternetAvailable();
            StartCoroutine(CheckCoroutine());
        }
    }

    IEnumerator CheckCoroutine()
    {
        while (true)
        {
            yield return WaitFor.Seconds(checkInterval);

            if (status != IsInternetAvailable())
            {
                status = IsInternetAvailable();
                OnNetworkStatusChanged?.Invoke();
            }
        }
    }

    public bool IsInternetAvailable()
    {
#if UNITY_EDITOR
        return isInternetAvailableOnEditor;
#else
        return !(Application.internetReachability == NetworkReachability.NotReachable);
#endif
    }
}