using System;
using System.Collections.Generic;

public static class ListUtil
{
    // Generate a list that contains the non-null elements of the source list.
    // Optionally inserts an extra element on the end.
    public static void Resize<T>(this List<T> self, int size, Func<T> createDefault)
    {
        if (size <= 0)
        {
            self.Clear();
            return;
        }

        if (size < self.Count)
        {
            self.RemoveRange(size, self.Count - size);
        }
        else if (size > self.Count)
        {
            int itemsToAdd = size - self.Count;
            for (int i = 0; i < itemsToAdd; ++i)
            {
                self.Add(createDefault());
            }
        }
    }

    private struct DelegateComparer<T> : IComparer<T>
    {
        LessThanDelegate<T> lessThanFunc;

        public DelegateComparer(LessThanDelegate<T> lessThanFunc)
        {
            this.lessThanFunc = lessThanFunc;
        }

        public int Compare(T x, T y)
        {
            // Note: Because we don't have a "0" case I'm not sure that this allows for stable sorting but honestly who actually cares about stable sorting?
            if (lessThanFunc(x, y))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    public delegate bool LessThanDelegate<T>(T a, T b);

    /// <summary>
    /// Like List.Sort but instead of passing in a Comparer we just pass in a function that returns true if the first argument is less than the second argument.
    /// </summary>
    /// <typeparam name="T">Type stored in array.</typeparam>
    /// <param name="arr">The List to sort.</param>
    /// <param name="lessThanFunc">A function that returns true if the first argument is less than the second argument and false otherwise.</param>
    public static void Sort<T>(List<T> list, LessThanDelegate<T> lessThanFunc)
    {
        list.Sort(new DelegateComparer<T>(lessThanFunc));
    }

    /// <summary>
    /// Performs an insertion sort on a List using the provided less than callback.
    /// Insertion sort is typically quite fast for mostly sorted data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="lessThanFunc"></param>
    public static void InsertionSort<T>(List<T> list, LessThanDelegate<T> lessThanFunc)
    {
        int i = 1;
        while (i < list.Count)
        {
            int j = i;
            while (j > 0 && lessThanFunc(list[j], list[j - 1]))
            {
                T temp = list[j];
                list[j] = list[j - 1];
                list[j - 1] = temp;

                --j;
            }
            ++i;
        }
    }

    // Like RemoveAll but stops after finding the first instance.
    public static bool RemoveFirst<T>(this List<T> list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    // Like RemoveFirst but starts at the back of the list and moves towards the front.
    public static bool RemoveLast<T>(this List<T> list, Predicate<T> predicate)
    {
        for (int i = list.Count - 1; i >= 0; --i)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public static T SelectRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}

// Used in rollback states for optimized cloning
public class CloneList<T> : List<T>
{
    public object ShallowClone()
    {
        return MemberwiseClone();
    }

    public CloneList() { }
    public CloneList(List<T> other) : base(other) { }
}
