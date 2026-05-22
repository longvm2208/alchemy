using UnityEngine;

public class ConfManager : SingletonMonoBehaviour<ConfManager>
{
    [SerializeReference] IConf[] confs;

    public T GetDefaultConf<T>() where T : IConf
    {
        for (int i = 0; i < confs.Length; i++)
        {
            if (confs[i] is T conf)
            {
                return conf;
            }
        }
        return default;
    }

#if UNITY_EDITOR
    public void SetConf<T>(IConf conf) where T : IConf
    {
        for (int i = 0; i < confs.Length; i++)
        {
            if (confs[i] is T)
            {
                confs[i] = conf;
            }
        }
    }
#endif
}
