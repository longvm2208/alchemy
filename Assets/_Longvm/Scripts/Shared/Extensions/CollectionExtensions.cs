using System.Collections.Generic;

public static class CollectionExtensions
{
    #region ILIST
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static T Get<T>(this IList<T> list, int index)
    {
        if (list == null || list.Count == 0) return default;
        int count = list.Count;
        return list[(index % count + count) % count];
    }

    public static void Set<T>(this IList<T> list, int index, T value)
    {
        if (list == null || list.Count == 0) return;
        int count = list.Count;
        list[(index % count + count) % count] = value;
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        (list[i], list[j]) = (list[j], list[i]);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new();
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void ShuffleRange<T>(this IList<T> list, int startIndex, int endIndex)
    {
        System.Random rng = new();

        for (int i = endIndex; i > startIndex; i--)
        {
            int j = rng.Next(startIndex, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static int GetWeightedRandomIndex(this IList<int> probabilities)
    {
        int totalWeight = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            totalWeight += probabilities[i];
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeWeight += probabilities[i];

            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }

        return UnityEngine.Random.Range(0, probabilities.Count);
    }
    #endregion

    #region LIST
    public static List<T> InitOrClear<T>(this List<T> list)
    {
        if (list == null)
        {
            list = new List<T>();
        }
        else
        {
            list.Clear();
        }

        return list;
    }

    public static void Rotate<T>(this List<T> list, int position)
    {
        int count = list.Count;
        position = (position % count + count) % count;
        if (position == 0) return;
        list.Reverse();
        list.Reverse(0, position);
        list.Reverse(position, count - position);
    }

    public static List<List<T>> GetCombinations<T>(this List<T> list, int k)
    {
        List<List<T>> combinations = new List<List<T>>();
        List<T> currentCombination = new List<T>();

        GenerateCombinations(list, k, 0, currentCombination, combinations);
        return combinations;
    }

    static void GenerateCombinations<T>(List<T> list, int k, int startIndex, List<T> currentCombination, List<List<T>> result)
    {
        if (currentCombination.Count == k)
        {
            result.Add(new List<T>(currentCombination));
            return;
        }

        for (int i = startIndex; i < list.Count; i++)
        {
            currentCombination.Add(list[i]);
            GenerateCombinations(list, k, i + 1, currentCombination, result);
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }

    public static IEnumerable<List<T>> GetCombinationsEnumerable<T>(this List<T> list, int k)
    {
        return GenerateCombinationsEnumerable(list, k, 0, new List<T>());
    }

    static IEnumerable<List<T>> GenerateCombinationsEnumerable<T>(List<T> list, int k, int startIndex, List<T> currentCombination)
    {
        if (currentCombination.Count == k)
        {
            yield return new List<T>(currentCombination);
            yield break;
        }

        for (int i = startIndex; i < list.Count; i++)
        {
            currentCombination.Add(list[i]);
            foreach (var combination in GenerateCombinationsEnumerable(list, k, i + 1, currentCombination))
            {
                yield return combination;
            }
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }
    #endregion

    public static bool IsNullOrEmpty<T>(this Queue<T> queue)
    {
        return queue == null || queue.Count == 0;
    }

    public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        return dictionary == null || dictionary.Count == 0;
    }
}
