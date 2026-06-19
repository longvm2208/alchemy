using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [PreviewField]
    public Sprite Icon;

    public ItemId Id;
    public string Name;
    public string Description;
    public CategoryId[] Categories;
}
