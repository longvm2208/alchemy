using System.Collections.Generic;
using UnityEngine;

public struct Pool<T> where T : ExtendedMonoBehaviour
{
    Queue<T> pool;
    List<T> actives;

    public T GetInstance(T prefab, Transform parent, bool setParent = false)
    {
        if (pool == null)
        {
            pool = new Queue<T>();
        }

        T instance;

        if (pool.Count > 0)
        {
            instance = pool.Dequeue();

            if (instance == null)
            {
                pool.Clear();
                instance = Object.Instantiate(prefab, parent);
            }
            else
            {
                instance.gameObject.SetActive(true);

                if (setParent && parent != null)
                {
                    instance.transform.SetParent(parent);
                }
            }
        }
        else
        {
            instance = Object.Instantiate(prefab, parent);
        }

        if (actives == null)
        {
            actives = new List<T>();
        }

        if (instance != null && !actives.Contains(instance))
        {
            actives.Add(instance);
        }

        return instance;
    }

    public void Recycle(T instance)
    {
        if (pool == null)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(instance.gameObject);
            }
            else
            {
                Object.DestroyImmediate(instance.gameObject);
            }
        }
        else
        {
            if (!pool.Contains(instance))
            {
                pool.Enqueue(instance);
            }

            instance.gameObject.SetActive(false);
        }

        if (actives != null && instance != null && actives.Contains(instance))
        {
            actives.Remove(instance);
        }
    }

    public void RecycleAll()
    {
        if (!actives.IsNullOrEmpty())
        {
            for (int i = 0; i < actives.Count; i++)
            {
                T instance = actives[i];

                if (!pool.Contains(instance))
                {
                    pool.Enqueue(instance);
                }

                instance.gameObject.SetActive(false);
            }
        }
    }
}
