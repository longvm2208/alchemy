using UnityEngine;

public abstract class EntryBase : PooledMonoBehaviour<EntryBase>
{
    public virtual void Init(int index)
    {

    }

    public virtual void Destroy()
    {
        Recycle();
    }
}
