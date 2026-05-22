using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Popups Container", menuName = "SO/Popups Container")]
public class PopupsContainer : ScriptableObject
{
#if UNITY_EDITOR
    [InlineButton(nameof(FindPrefabs), "Find")]
#endif
    [SerializeField] List<Popup> prefabs;

    public Popup this[int index]
    {
        get
        {
            if (prefabs == null || prefabs.Count == 0)
            {
                Debug.LogError("Prefabs list is empty");
                return null;
            }
            if (index < 0 || index >= prefabs.Count)
            {
                Debug.LogError($"{index} is out of range");
                return null;
            }
            return prefabs[index];
        }
    }

    public int Count => prefabs?.Count ?? 0;

#if UNITY_EDITOR
    void FindPrefabs()
    {
        if (prefabs == null) prefabs = new();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Popup prefab = AssetDatabase.LoadAssetAtPath<Popup>(path);
            if (prefab == null) continue;
            if (!prefabs.Contains(prefab)) prefabs.Add(prefab);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
}
