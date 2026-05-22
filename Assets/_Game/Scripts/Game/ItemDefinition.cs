using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [PreviewField]
    public Sprite Icon;

    public ItemId Id;
    public string Name;
    public string Description;

#if UNITY_EDITOR
    [Button]
    void Setup()
    {
        Id = Enum.Parse<ItemId>(name);
        Name = name;
    }
#endif
}
