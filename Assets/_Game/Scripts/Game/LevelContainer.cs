using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "SO/Game/Level Container")]
public class LevelContainer : ScriptableObject
{
    public LevelData[] Levels;
}