using Sirenix.OdinInspector;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item", menuName = "SO/Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [PreviewField]
    public Sprite Icon;

    public ItemId Id;
    public string Name;
    public string Description;
    public int Tier;
    public GroupId GroupId;
    public BranchId BranchId;

#if UNITY_EDITOR
    [Button]
    void Setup()
    {
        Id = Enum.Parse<ItemId>(name);
        Name = name;
    }

    [Button]
    void UpdateName()
    {
        string path = AssetDatabase.GetAssetPath(this);

        if (!string.IsNullOrEmpty(path))
        {
            string group = GroupId == GroupId.None ? GroupId.ToString() : "Basic";

            string name = $"{GroupId}-{BranchId}-{Tier}-{Id}";
            AssetDatabase.RenameAsset(path, name);
        }
    }
#endif
}
