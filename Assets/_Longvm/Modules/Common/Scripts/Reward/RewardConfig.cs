using Sirenix.OdinInspector;
using System;

[Serializable]
public struct RewardConfig
{
    [HorizontalGroup("row"), HideLabel] public RewardType Type;
    [HorizontalGroup("row"), HideLabel] public int Count;

    public RewardConfig(RewardType type, int count)
    {
        Type = type;
        Count = count;
    }

    public string GetCountString()
    {
        switch (Type)
        {
            case RewardType.UnlimitedEnergy:
                return Count.ToStringReward();
            case RewardType.Magic:
            case RewardType.Sweep:
            case RewardType.Magnet:
                return $"x{Count}";
            default:
                return Count.ToString();
        }
    }

    public string GetFloating()
    {
        switch (Type)
        {
            case RewardType.UnlimitedEnergy:
                return Count.ToStringReward();
            default:
                return Count.ToString();
        }
    }
}
