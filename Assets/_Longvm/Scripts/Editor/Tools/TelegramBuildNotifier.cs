using System.IO;
using System.Net;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Networking;

public class TelegramBuildNotifier : IPostprocessBuildWithReport
{
    const string BotTokenFilePath = @"D:\Secrets\telegram_bot_token.txt";
    const string ChatIdFilePath = @"D:\Secrets\telegram_chat_id.txt";

    public int callbackOrder => 999;

    public void OnPostprocessBuild(BuildReport report)
    {
        string message =
            $"Build Completed\n" +
            $"Project: {Application.productName}\n" +
            $"Platform: {report.summary.platform}\n" +
            $"Time: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}";

        string botToken = ReadFromFile(BotTokenFilePath);
        string chatId = ReadFromFile(ChatIdFilePath);

        SendTelegramMessage(botToken, chatId, message);
    }

    void SendTelegramMessage(string botToken, string chatId, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId)) return;

            string url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string payload = $"chat_id={chatId}&text={UnityWebRequestEscape(message)}&parse_mode=Markdown";
                client.UploadString(url, payload);
            }

            Debug.Log("[TelegramBuildNotifier] Telegram notification sent successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[TelegramBuildNotifier] Failed to send Telegram message: " + ex);
        }
    }

    string UnityWebRequestEscape(string s)
    {
        return UnityWebRequest.EscapeURL(s).Replace("+", "%20");
    }

    string ReadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[TelegramBuildNotifier] File not found: {path}");
                return null;
            }

            using (StreamReader reader = new(path))
            {
                string line = reader.ReadLine();
                if (line == null) return null;
                return line.Trim();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[TelegramBuildNotifier] Error while reading file: {ex}");
            return null;
        }
    }
}
