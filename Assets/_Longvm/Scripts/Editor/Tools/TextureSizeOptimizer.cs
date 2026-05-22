using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureSizeOptimizer
{
    [MenuItem("Tools/My Tools/Texture Size Optimizer/Trim Transparent Border")]
    static void TrimTransparentBorder()
    {
        Object[] objects = Selection.objects;
        foreach (var obj in objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Texture2D texture = obj as Texture2D;

            if (texture == null)
            {
                Debug.LogWarning($"Selected object '{obj.name}' is not a Texture2D");
                continue;
            }

            importer.isReadable = true;
            importer.SaveAndReimport();

            SaveTexture(TrimTransparentBorder(texture), path);

            importer.isReadable = false;
            importer.SaveAndReimport();
        }
    }

    [MenuItem("Tools/My Tools/Texture Size Optimizer/Width And Height Being Multiple Of 4")]
    static void AddTransparentBorder()
    {
        Object[] objects = Selection.objects;
        foreach (var obj in objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Texture2D texture = obj as Texture2D;

            if (texture == null)
            {
                Debug.LogWarning($"Selected object '{obj.name}' is not a Texture2D");
                continue;
            }

            importer.isReadable = true;
            importer.SaveAndReimport();

            SaveTexture(AddTransparentBorder(texture), path);

            importer.isReadable = false;
            importer.SaveAndReimport();
        }
    }

    [MenuItem("Tools/My Tools/Texture Size Optimizer/Trim Transparent Border", true)]
    [MenuItem("Tools/My Tools/Texture Size Optimizer/Width And Height Being Multiple Of 4", true)]
    static bool Validation()
    {
        Object[] objects = Selection.objects;

        if (objects == null || objects.Length == 0) return false;

        foreach (Object obj in objects)
        {
            if (obj != null && obj.GetType() == typeof(Texture2D))
            {
                return true;
            }
        }

        return false;
    }

    static Texture2D TrimTransparentBorder(Texture2D originalTexture)
    {
        int left = originalTexture.GetTransparentBorderLeftSize();
        int right = originalTexture.GetTransparentBorderRightSize();
        int bottom = originalTexture.GetTransparentBorderBottomSize();
        int top = originalTexture.GetTransparentBorderTopSize();

        int width = originalTexture.width - left - right;
        int height = originalTexture.height - bottom - top;

        Texture2D newTexture = new Texture2D(width, height);

        for (int x = left; x < originalTexture.width - right; x++)
        {
            for (int y = bottom; y < originalTexture.height - top; y++)
            {
                Color color = originalTexture.GetPixel(x, y);
                newTexture.SetPixel(x - left, y - bottom, color);
            }
        }

        newTexture.Apply();
        newTexture.name = originalTexture.name;

        return newTexture;
    }

    static Texture2D AddTransparentBorder(Texture2D originalTexture)
    {
        int newWidth = (originalTexture.width + 3) / 4 * 4;
        int newHeight = (originalTexture.height + 3) / 4 * 4;

        Texture2D newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

        Color32[] transparentColors = new Color32[newWidth * newHeight];
        for (int i = 0; i < transparentColors.Length; i++)
        {
            transparentColors[i] = new Color32(0, 0, 0, 0);
        }

        newTexture.SetPixels32(transparentColors);

        int left = (newWidth - originalTexture.width) / 2;
        int bottom = (newHeight - originalTexture.height) / 2;
        newTexture.SetPixels(left, bottom, originalTexture.width, originalTexture.height, originalTexture.GetPixels());

        newTexture.Apply();
        newTexture.name = originalTexture.name;

        return newTexture;
    }

    static void SaveTexture(Texture2D texture, string path)
    {
        File.WriteAllBytes(path, texture.EncodeToPNG());
    }
}

#region TEXTURE 2D EXTENSIONS
public static class Texture2DExtensions
{
    public static int GetTransparentBorderLeftSize(this Texture2D texture)
    {
        bool isBreak = false;
        int size = 0;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).a > 0f)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak) break;

            size++;
        }

        return size;
    }

    public static int GetTransparentBorderRightSize(this Texture2D texture)
    {
        bool isBreak = false;
        int size = 0;

        for (int x = texture.width - 1; x >= 0; x--)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).a > 0f)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak) break;

            size++;
        }

        return size;
    }

    public static int GetTransparentBorderBottomSize(this Texture2D texture)
    {
        bool isBreak = false;
        int size = 0;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (texture.GetPixel(x, y).a > 0f)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak) break;

            size++;
        }

        return size;
    }

    public static int GetTransparentBorderTopSize(this Texture2D texture)
    {
        bool isBreak = false;
        int size = 0;

        for (int y = texture.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (texture.GetPixel(x, y).a > 0f)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak) break;

            size++;
        }

        return size;
    }
}
#endregion
