using System;
using System.Collections.Generic;
using UnityEngine;

public class SSOManager : SingletonMonoBehaviour<SSOManager>
{
    [SerializeField] List<ScriptableObject> scriptableObjects;

    public ScriptableObject GetObject(Type type)
    {
        for (int i = 0; i < scriptableObjects.Count; i++)
        {
            if (scriptableObjects[i].GetType() == type)
            {
                return scriptableObjects[i];
            }
        }

        return null;
    }

    public void SetObject(ScriptableObject obj)
    {
        if (!scriptableObjects.Contains(obj))
        {
            scriptableObjects.Add(obj);
        }
    }
}
