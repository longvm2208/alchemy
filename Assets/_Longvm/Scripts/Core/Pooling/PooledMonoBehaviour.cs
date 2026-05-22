using UnityEngine;

public class PooledMonoBehaviour<T> : ExtendedMonoBehaviour where T : PooledMonoBehaviour<T>
{
    Pool<T> pool;

    public T GetInstance(Transform parent, bool setParent = false)
    {
        T instance = pool.GetInstance((T)this, parent, setParent);
        instance.pool = pool;
        return instance;
    }

    public virtual void Recycle()
    {
        pool.Recycle((T)this);
    }
}
