using Corelibs.Basic.Maths;
using System;
using System.Collections.Generic;

namespace Corelibs.Basic.Collections
{
    public static class ListExtensions
    {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public static void SwapFirst<T>(this IList<T> list, int index) => list.Swap(index, 0);
        public static void SwapLast<T>(this IList<T> list, int index) => list.Swap(index, list.Count - 1);
        public static void SwapFirstAndLast<T>(this IList<T> list) => list.SwapLast(0);

        public static void MoveToLast<T>(this IList<T> list, int index)
        {
            list.Add(list[index]);
            list.RemoveAt(index);
        }

        public static void MoveFirstToLast<T>(this IList<T> list) => list.MoveToLast(0);

        public static void InsertClamped<T>(this List<T> list, T item, int index)
        {
            index = index.Clamp(list.Count);
            list.Insert(index, item);
        }

        public static void InsertForced<T>(this List<T> list, T item, int index)
        {
            list.AddRange(index + 1 - list.Count);
            list[index] = item;
        }

        public static void AddRange<T>(this List<T> list, int count)
        {
            if (count <= 0)
                return;

            list.AddRange(new T[count]);
        }

        public static T GetItemAfter<T>(this IList<T> list, T item) => list.GetItemNFrom(item, 1);

        public static T GetItemBefore<T>(this IList<T> list, T item) => list.GetItemNFrom(item, -1);

        public static T GetItemNFrom<T>(this IList<T> list, T item, int indexOffset)
        {
            int index = list.IndexOf(item);
            if (!list.IsIndexInRange(index))
                return default;

            int newIndex = index + indexOffset;
            if (!list.IsIndexInRange(newIndex))
                return default;

            return list[newIndex];
        }

        public static void RemoveFirstIf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (!predicate(item))
                    continue;

                list.RemoveAt(i);
                return;
            }
        }

        public static bool RemoveIf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            bool removed = false;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                if (!predicate(item))
                    continue;

                list.RemoveAt(i);
                removed = true;
            }

            return removed;
        }

        public static T AddTo<T>(this T item, IList<T> list)
        {
            if (list == null || item == null)
                return item;

            list.Add(item);
            return item;
        }

        public static void AddIfNotEmpty<T>(this IList<T> list, T item) 
            where T : IEnumerable<object>
        {
            if (item.Count() > 0)
                list.Add(item);
        }

        public static T Get<T>(
            this IList<T> list, Func<T, bool> keySelector, 
            int index,
            out int foundAtIndex)
        {
            foundAtIndex = index;
            if (list.IsNullOrEmpty())
                return default;

            if (list.IsIndexInRange(index))
                return list[index];

            T item = list.FirstOrDefault(keySelector);
            if (item == null)
                return default;

            foundAtIndex = list.IndexOf(item);

            return item;
        }
    }
}
