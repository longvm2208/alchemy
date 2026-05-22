using UnityEditor;

public class ClearEditorPrefs
{
    [MenuItem("Tools/My Tools/Clear All EditorPrefs")]
    static void Clear()
    {
        if (EditorUtility.DisplayDialog("Clear All EditorPrefs",
            "Are you sure you want to clear all EditorPrefs? " +
            "This action cannot be undone.", "Yes", "No"))
        {
            EditorPrefs.DeleteAll();
        }
    }
}
