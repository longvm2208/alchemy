using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "SO/Game/Level Data")]
public class LevelData : ScriptableObject
{
    public ItemId[] StartItems;
    [TableList]
    public LevelTarget[] Targets;
}

[Serializable]
public struct LevelTarget
{
    public int RequiredAmount;
}
