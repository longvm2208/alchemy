using DependenciesHunter;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "ItemDatabaseConfig", menuName = "SO/Game/Item Database Config")]
public class ItemDatabaseConfig : ScriptableObject
{
    public ItemDefinition[] Items;
    [TableList]
    public MergeRecipe[] Recipes;

#if UNITY_EDITOR
    [Serializable]
    class ItemsSheet
    {
        public ItemsSheetRow[] Rows;
    }

    [Serializable]
    class ItemsSheetRow
    {
        public string Name;
        public string Description;
    }

    [Serializable]
    class MergeMapSheet
    {
        public MergeMapSheetRow[] Rows;
    }

    [Serializable]
    class MergeMapSheetRow
    {
        public string Id;
        public string ItemAId;
        public string ItemBId;
        public string ResultItemId;
    }

    [Header("Editor")]
    [SerializeField] string appsScriptUrl;
    [SerializeField, Sirenix.OdinInspector.FilePath] string enumFilePath;
    [SerializeField, FolderPath] string itemFolderPath;

    [Button]
    async void UpdateItemIds()
    {
        string url = appsScriptUrl + "?type=Items";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<ItemsSheet>(json);

            List<string> itemNames = new List<string>();
            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                itemNames.Add(sheet.Rows[i].Name);
            }

            EnumGenerator.AppendNewEnumValues(enumFilePath, itemNames);

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }
    }

    [Button]
    async void UpdateItems()
    {
        string url = appsScriptUrl + "?type=Items";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            var sheet = JsonUtility.FromJson<ItemsSheet>(json);

            Dictionary<ItemId, ItemDefinition> itemDict = new();

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;
                itemDict.Add(Items[i].Id, Items[i]);
            }

            List<ItemDefinition> items = new();

            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                ItemsSheetRow row = sheet.Rows[i];
                ItemId id = Enum.Parse<ItemId>(row.Name);

                if (itemDict.ContainsKey(id))
                {
                    ItemDefinition itemDef = itemDict[id];

                    if (string.Compare(itemDef.Description, row.Description) != 0)
                    {
                        itemDef.Description = row.Description;
                        EditorUtility.SetDirty(itemDef);
                    }

                    items.Add(itemDef);
                }
                else
                {
                    ItemDefinition itemDef = CreateInstance<ItemDefinition>();
                    itemDef.Id = id;
                    itemDef.Name = row.Name;
                    itemDef.Description = row.Description;

                    string assetPath = Path.Combine(itemFolderPath, $"{row.Name}.asset");

                    AssetDatabase.CreateAsset(itemDef, assetPath);

                    items.Add(itemDef);
                }
            }

            Items = items.ToArray();
            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }
    }

    [Button]
    async void UpdateRecipes()
    {
        string url = appsScriptUrl + "?type=MergeMap";

        UnityWebRequest request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";

            var sheet = JsonUtility.FromJson<MergeMapSheet>(json);

            Recipes = new MergeRecipe[sheet.Rows.Length];

            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                Recipes[i] = new MergeRecipe
                {
                    Id = int.Parse(sheet.Rows[i].Id),
                    ItemAId = Enum.Parse<ItemId>(sheet.Rows[i].ItemAId),
                    ItemBId = Enum.Parse<ItemId>(sheet.Rows[i].ItemBId),
                    ResultItemId = Enum.Parse<ItemId>(sheet.Rows[i].ResultItemId)
                };
            }

            Debug.Log("Load Success: " + json);
        }
        else
        {
            Debug.LogError("Load Failed: " + request.error);
        }
    }
#endif
}
