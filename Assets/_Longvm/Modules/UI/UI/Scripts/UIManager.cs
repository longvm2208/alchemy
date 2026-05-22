using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] RectTransform popupParent;
    [SerializeField] GameObject inputBlocker;
    [SerializeField, ExposedScriptableObject] PopupsContainer popupsContainer;
    [SerializeField] TransitionAnimationBase transitionAnimation;
    public TransitionAnimationBase TransitionAnimation => transitionAnimation;

    int openedPopupCount = 0;
    public bool IsAnyPopupOpen => openedPopupCount > 0;
    readonly Dictionary<Type, Popup> popupPrefabs = new();
    readonly Dictionary<Type, Popup> popupInstances = new();

    public void Init()
    {
        for (int i = 0; i < popupsContainer.Count; i++)
        {
            popupPrefabs[popupsContainer[i].GetType()] = popupsContainer[i];
        }
    }

    #region POPUP
    public void OnPopupOpen()
    {
        openedPopupCount++;
    }

    public void OnPopupClose()
    {
        openedPopupCount--;
    }

    public bool HasPrefab<T>() where T : Popup
    {
        return HasPrefab(typeof(T));
    }

    bool HasPrefab(Type type)
    {
        return popupPrefabs.ContainsKey(type) && popupPrefabs[type] != null;
    }

    public bool HasInstance<T>() where T : Popup
    {
        return HasInstance(typeof(T));
    }

    bool HasInstance(Type type)
    {
        return popupInstances.ContainsKey(type) && popupInstances[type] != null;
    }

    public bool IsOpen<T>() where T : Popup
    {
        return IsOpen(typeof(T));
    }

    bool IsOpen(Type type)
    {
        return HasInstance(type) && popupInstances[type].gameObject.activeSelf;
    }

    public T Open<T>() where T : Popup
    {
        Type type = typeof(T);
        return Open(type) as T;
    }

#if UNITY_EDITOR
    IEnumerable<Type> GetPopupTypes()
    {
        return typeof(Popup).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Popup)) && !t.IsAbstract && !t.IsGenericType);
    }

    [Button(ButtonStyle.FoldoutButton)]
    Popup Open([ValueDropdown(nameof(GetPopupTypes))] Type type)
#else
    Popup Open(Type type)
#endif
    {
        if (!IsOpen(type))
        {
            if (!HasInstance(type))
            {
                if (!HasPrefab(type))
                {
                    Debug.LogError("Prefab not found: " + type.FullName);
                    return null;
                }
                popupInstances[type] = Instantiate(popupPrefabs[type], popupParent);
            }
            popupInstances[type].Open();
        }
        return popupInstances[type];
    }

    public T Get<T>() where T : Popup
    {
        return Get(typeof(T)) as T;
    }

    Popup Get(Type type)
    {
        if (HasInstance(type))
        {
            return popupInstances[type];
        }
        else
        {
            return null;
        }
    }

    public void Close<T>() where T : Popup
    {
        Type type = typeof(T);
        if (IsOpen(type))
        {
            popupInstances[type].Close();
        }
    }

    public void CloseAll()
    {
        if (!IsAnyPopupOpen) return;
        foreach (var popup in popupInstances.Values)
        {
            if (popup != null && popup.gameObject.activeSelf)
            {
                popup.Disable();
            }
        }
    }
    #endregion

    #region INPUT BLOCKER
    public void EnableBlocker(bool enable)
    {
        inputBlocker?.SetActive(enable);
    }
    #endregion
}