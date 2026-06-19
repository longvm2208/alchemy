using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameConf : ConfBase<GameConf>
{
    public int HintLevel;
    public int HintPrice;
    public int ExtraTimeLevel;
    public int ExtraTimePrice;
    [TableList]
    public TimeLimit[] TimeLimits;

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] string time;

    [Button]
    void SetTime()
    {
        string[] ts = time.Split(',');

        TimeLimits = new TimeLimit[ts.Length];

        for (int i = 0; i < ts.Length; i++)
        {
            if (int.TryParse(ts[i], out int t))
            {
                TimeLimits[i] = new TimeLimit()
                {
                    Level = i + 1,
                    Time = t
                };
            }
            else
            {
                Debug.LogError("Invalid time");
            }
        }
    }
#endif
}

[Serializable]
public struct TimeLimit
{
    public int Level;
    public int Time;
}
