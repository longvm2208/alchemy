using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class PrefManager : SingletonMonoBehaviour<PrefManager>
{
#if UNITY_EDITOR
    [SerializeReference]
#endif
    List<IPref> prefs = new();

    public event Action OnAppQuit;

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        OnAppQuit?.Invoke();

        for (int i = 0; i < prefs.Count; i++)
        {
            prefs[i].OnAppQuit();
        }
        Save();
    }

    public void OnPrefLoaded(IPref pref)
    {
        if (!prefs.Contains(pref))
        {
            prefs.Add(pref);
        }
    }

    void Save()
    {
        for (int i = 0; i < prefs.Count; i++)
        {
            prefs[i].Save();
        }
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [ValueDropdown(nameof(GetPrefTypes))]
    [SerializeReference] Type type;
    [InlineButton(nameof(ClearPref), "Clear")]
    [InlineButton(nameof(SavePref), "Save")]
    [InlineButton(nameof(LoadPref), "Load")]
    [SerializeReference] IPref pref;

    IEnumerable<Type> GetPrefTypes()
    {
        return typeof(IPref).Assembly.GetTypes()
            .Where(t => typeof(IPref).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
    }

    void LoadPref()
    {
        pref = PrefUtils.Load(type);
    }

    void SavePref()
    {
        PrefUtils.Save(pref);
        ClearPref();
    }

    void ClearPref()
    {
        pref = null;
        type = null;
    }
#endif
}
