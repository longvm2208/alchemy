using Sirenix.OdinInspector;
using System;
using System.Text;
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
    class SheetObject
    {
        public SheetRow[] Rows;
    }

    [Serializable]
    class SheetRow
    {
        public string ItemAId;
        public string ItemBId;
        public string ResultItemId;
    }

    [Header("Editor")]
    [InlineButton(nameof(LoadFromGoogleSheet), "Load")]
    [InlineButton(nameof(UploadToGoogleSheet), "Upload")]
    [SerializeField] string appsScriptUrl;

    async void LoadFromGoogleSheet()
    {
        UnityWebRequest request = UnityWebRequest.Get(appsScriptUrl);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            json = "{\"Rows\":" + json + "}";
            
            var sheet = JsonUtility.FromJson<SheetObject>(json);

            Recipes = new MergeRecipe[sheet.Rows.Length];

            for (int i = 0; i < sheet.Rows.Length; i++)
            {
                Recipes[i] = new MergeRecipe
                {
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

    async void UploadToGoogleSheet()
    {
        SheetRow[] rows = new SheetRow[Recipes.Length];

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = new SheetRow
            {
                ItemAId = Recipes[i].ItemAId.ToString(),
                ItemBId = Recipes[i].ItemBId.ToString(),
                ResultItemId = Recipes[i].ResultItemId.ToString()
            };
        }

        SheetObject sheet = new SheetObject
        {
            Rows = rows
        };

        string json = JsonUtility.ToJson(sheet);

        Debug.Log("Upload JSON: " + json);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(appsScriptUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload Success");
        }
        else
        {
            Debug.LogError("Upload Error: " + request.error);
        }
    }
#endif
}
