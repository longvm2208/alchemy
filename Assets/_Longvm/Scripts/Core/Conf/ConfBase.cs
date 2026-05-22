using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ConfBase<T> : IConf where T : ConfBase<T>, new()
{
    static T ins;
    public static T Ins
    {
        get
        {
            if (ins == null)
            {
                if (ConfManager.Ins != null)
                {
                    ins = ConfManager.Ins.GetDefaultConf<T>();
                }
                else
                {
                    Debug.LogError("ConfManager is not initialized yet!");
                }

                if (ins == null)
                {
                    ins = new();
                }
            }

            return ins;
        }
    }

    public void SetIns(IConf conf)
    {
        if (conf == null) return;
        ins = conf as T;

#if UNITY_EDITOR
        ConfManager.Ins.SetConf<T>(conf);
#endif
    }

    [Button]
    void CopySerializedString()
    {
        GUIUtility.systemCopyBuffer = JsonUtility.ToJson(this);
    }
}
