using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AutoKeystorePasswordSetup : IPreprocessBuildWithReport
{
    const string PasswordFilePath = @"D:\Secrets\block_sort_password.txt";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_ANDROID
        string password = ReadPasswordFromFile(PasswordFilePath);
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError($"[AutoKeystorePasswordSetup] Failed to read password from '{PasswordFilePath}'. Build aborted.");
            throw new System.Exception("Keystore password not found.");
        }

        PlayerSettings.Android.keystorePass = password;
        PlayerSettings.Android.keyaliasPass = password;

        Debug.Log("[AutoKeystorePasswordSetup] Successfully loaded keystore password from file and applied PlayerSettings."); 
#endif
    }

    string ReadPasswordFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[AutoKeystorePasswordSetup] Password file not found: {path}");
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
            Debug.LogError($"[AutoKeystorePasswordSetup] Error while reading password file: {ex}");
            return null;
        }
    }
}
