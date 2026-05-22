using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public struct NewDayEvent { }

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    [SerializeField] bool isLocalTime;
    [SerializeField] int timeout = 3;
    [SerializeField] string url = "https://www.google.com";
    [Space]
    [SerializeField] float deltaTime = 0.2f;

    float timeCounter = -1;
    DateTime startupTime = DateTime.Now;
    public DateTime Now => (isLocalTime || !IsFetched) ? DateTime.Now : startupTime.AddSeconds(Time.realtimeSinceStartup);

    public bool IsFetched { get ; private set; }

    private void Update()
    {
        CheckNewDay();
    }

    public void FetchTime()
    {
        if (isLocalTime)
        {
            IsFetched = true;
        }
        else
        {
            try
            {
                StartCoroutine(FetchTimeCoroutine());
            }
            catch (Exception e)
            {
                Debug.LogError("Fetch time from server failed: " + e.Message);
                startupTime = DateTime.Now - TimeSpan.FromSeconds(Time.realtimeSinceStartup);
                IsFetched = true;
            }
        }
    }

    IEnumerator FetchTimeCoroutine()
    {
        DateTime time;
        UnityWebRequest request = new UnityWebRequest(url);
        request.timeout = timeout;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Fetch time from server successfully");
            string date = request.GetResponseHeaders()["date"];
            time = DateTime.Parse(date);
        }
        else
        {
            Debug.LogWarning("Using local time");
            time = DateTime.Now;
        }

        startupTime = time - TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        IsFetched = true;
    }

    void CheckNewDay()
    {
        if (!IsFetched) return;

        if (timeCounter > 0)
        {
            timeCounter -= Time.unscaledDeltaTime;
            return;
        }

        DateTime today = Now.Date;
        if (TimePref.Ins.LastDate.Value < today)
        {
            // New Day
            TimePref.Ins.LastDate.Value = today;
            EventBus.Publish<NewDayEvent>(default);
        }

        timeCounter = deltaTime;
    }
}
