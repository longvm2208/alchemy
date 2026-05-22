using System;
using UnityEngine;

public class PrefUtils
{
    public static T Load<T>() where T : IPref
    {
        return (T)Load(typeof(T));
    }

    public static IPref Load(Type type)
    {
        IPref pref;
        string key = type.FullName;

        if (!PlayerPrefs.HasKey(key))
        {
            pref = (IPref)Activator.CreateInstance(type);
            pref.Init();
        }
        else
        {
            string json = PlayerPrefs.GetString(key);
            pref = (IPref)JsonUtility.FromJson(json, type);
        }

        return pref;
    }

    public static void Save(IPref pref)
    {
        if (pref == null) return;
        string key = pref.GetType().FullName;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(pref));
    }
}
