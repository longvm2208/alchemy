using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableHashSet<T> : ISerializationCallbackReceiver, IEnumerable<T>
{
    [SerializeField]
    private List<T> items = new List<T>();

    private HashSet<T> hashSet = new HashSet<T>();

    public int Count => hashSet.Count;

    public bool Add(T item)
    {
        if (hashSet.Add(item))
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    public bool Remove(T item)
    {
        if (hashSet.Remove(item))
        {
            items.Remove(item);
            return true;
        }
        return false;
    }

    public bool Contains(T item)
    {
        return hashSet.Contains(item);
    }

    public void Clear()
    {
        hashSet.Clear();
        items.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return hashSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void OnBeforeSerialize()
    {
        hashSet.Clear();
        foreach (var item in items)
        {
            hashSet.Add(item);
        }

        items = new List<T>(hashSet);
    }

    public void OnAfterDeserialize()
    {
        hashSet = new HashSet<T>(items);
    }
}