#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EnumGenerator
{
    public static void UpdateEnum(string filePath, string enumName, List<string> values)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public enum " + enumName);
        sb.AppendLine("{");

        sb.AppendLine("    None = 0,");

        for (int i = 0; i < values.Count; i++)
        {
            string value = SanitizeEnumName(values[i]);

            sb.AppendLine($"    {value} = {i + 1},");
        }

        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString());

        AssetDatabase.Refresh();

        Debug.Log($"Updated enum: {enumName}");
    }

    public static void AppendNewEnumValues(
        string filePath,
        List<string> newValues)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Enum file not found: {filePath}");
            return;
        }

        string content = File.ReadAllText(filePath);

        // Parse enum entries
        Regex regex = new Regex(@"(\w+)\s*=\s*(\d+)");
        MatchCollection matches = regex.Matches(content);

        Dictionary<string, int> existing = new();

        int maxValue = -1;

        foreach (Match match in matches)
        {
            string name = match.Groups[1].Value;
            int value = int.Parse(match.Groups[2].Value);

            existing[name] = value;

            if (value > maxValue)
                maxValue = value;
        }

        List<string> toAdd = new();

        foreach (string raw in newValues)
        {
            string name = SanitizeEnumName(raw);

            if (!existing.ContainsKey(name))
            {
                maxValue++;
                toAdd.Add($"    {name} = {maxValue},");
            }
        }

        if (toAdd.Count == 0)
        {
            Debug.Log("No new enum values.");
            return;
        }

        // insert before last }
        int lastBraceIndex = content.LastIndexOf('}');

        string insertText =
            string.Join("\n", toAdd) + "\n";

        content = content.Insert(lastBraceIndex, insertText);

        File.WriteAllText(filePath, content);

        AssetDatabase.Refresh();

        Debug.Log($"Added {toAdd.Count} new enum values.");
    }

    static string SanitizeEnumName(string input)
    {
        input = input.Replace(" ", "");
        input = input.Replace("-", "_");

        return input;
    }
}
#endif