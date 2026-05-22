using System.IO;
using UnityEditor;
using UnityEngine;

public class ReadWriteModifier
{
    static string True = "m_IsReadable: 1";
    static string False = "m_IsReadable: 0";

    [MenuItem("Tools/My Tools/Read Write Modifier/Modify To False")]
    static void ModifyToFalse()
    {
        Object[] objects = Selection.objects;
        foreach (Object obj in objects)
        {
            if (obj != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string[] lines = File.ReadAllLines(assetPath);
                bool modified = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(True))
                    {
                        lines[i] = lines[i].Replace(True, False);
                        modified = true;
                    }
                }

                if (modified)
                {
                    File.WriteAllLines(assetPath, lines);
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    [MenuItem("Tools/My Tools/Read Write Modifier/Modify To False", true)]
    static bool ModifyToFalseValidation()
    {
        Object[] objects = Selection.objects;

        if (objects == null || objects.Length == 0) return false;

        foreach (Object obj in objects)
        {
            if (obj != null && AssetDatabase.GetAssetPath(obj).EndsWith(".asset"))
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string[] lines = File.ReadAllLines(assetPath);
                foreach (string line in lines)
                {
                    if (line.Contains(True))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [MenuItem("Tools/My Tools/Read Write Modifier/Modify To True")]
    static void ModifyToTrue()
    {
        Object[] objects = Selection.objects;
        foreach (Object obj in objects)
        {
            if (obj != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string[] lines = File.ReadAllLines(assetPath);
                bool modified = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(False))
                    {
                        lines[i] = lines[i].Replace(False, True);
                        modified = true;
                    }
                }

                if (modified)
                {
                    File.WriteAllLines(assetPath, lines);
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    [MenuItem("Tools/My Tools/Read Write Modifier/Modify To True", true)]
    static bool ModifyToTrueValidation()
    {
        Object[] objects = Selection.objects;

        if (objects == null || objects.Length == 0) return false;

        foreach (Object obj in objects)
        {
            if (obj != null && AssetDatabase.GetAssetPath(obj).EndsWith(".asset"))
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string[] lines = File.ReadAllLines(assetPath);
                foreach (string line in lines)
                {
                    if (line.Contains(False))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
