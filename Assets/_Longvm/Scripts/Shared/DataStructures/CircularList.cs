using System;
using System.Collections.Generic;

[Serializable]
public class CircularList<T> : List<T>
{
    public new T this[int index]
    {
        get
        {
            if (Count == 0) return default;
            index = (index % Count + Count) % Count; // Normalize index to be non-negative
            return base[index];
        }

        set
        {
            if (Count == 0) return;
            index = (index % Count + Count) % Count; // Normalize index to be non-negative
            base[index] = value;
        }
    }
}
