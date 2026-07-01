using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] ProgressBar progressBar;
    [SerializeField] TMP_Text timeText;
    [SerializeField] float timeRemaining;
    public float TimeRemaining => timeRemaining;

    bool isRunning;
    float maxTime;

    private void Update()
    {
        if (!isRunning) return;

        if (UIManager.Ins.IsAnyPopupOpen) return;

        if (timeRemaining < 0)
        {
            Stop();
            UIManager.Ins.Open<PopupLose>();
        }
        else
        {
            timeRemaining -= Time.deltaTime;
            timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();
            progressBar.SetProgress(timeRemaining / maxTime);
        }
    }

    public void Init(int levelIndex)
    {
        isRunning = false;
        timeRemaining = GameConf.Ins.TimeLimits[levelIndex].Time;
        timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();

        maxTime = timeRemaining;
        progressBar.SetProgress(timeRemaining / maxTime);
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Revive()
    {
        timeRemaining = 60;
        timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();
        progressBar.SetProgress(timeRemaining / maxTime);
        Play();
    }

    public void AddTime(int time)
    {
        timeRemaining += time;
        timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();
        progressBar.SetProgress(timeRemaining / maxTime);
    }
}
