using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "SO/Game/Category Definition")]
public class CategoryDefinition : ScriptableObject
{
    [PreviewField]
    public Sprite Icon;

    public CategoryId Id;
    public string Name;
    public ItemId[] Items;
}
