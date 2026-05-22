using System;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T ins;
    public static T Ins
    {
        get
        {
            if (ins == null)
            {
                Type type = typeof(T);

                if (SSOManager.Ins)
                {
                    ins = SSOManager.Ins.GetObject(type) as T;
                }

                if (ins == null)
                {
                    ins = Resources.Load(type.Name) as T;

                    Debug.Log("Resources load");

                    if (ins != null && SSOManager.Ins)
                    {
                        SSOManager.Ins.SetObject(ins);
                    }
                }
            }

            return ins;
        }
    }
}
