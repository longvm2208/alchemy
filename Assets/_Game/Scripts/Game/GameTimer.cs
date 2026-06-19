using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    [SerializeField] float timeRemaining;
    public float TimeRemaining => timeRemaining;

    bool isRunning;

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
        }
    }

    public void Init(int levelIndex)
    {
        isRunning = false;
        timeRemaining = GameConf.Ins.TimeLimits[levelIndex].Time;
        timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();
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
        Play();
    }

    public void AddTime(int time)
    {
        timeRemaining += time;
        timeText.text = Mathf.CeilToInt(timeRemaining).ToStringCountdown();
    }
}
