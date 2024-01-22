using Corelibs.Basic.Functional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Corelibs.Basic.Collections
{
    public static class EnumerableExtensions
    {
        public static void ForEach(this int index, Action action)
        {
            index = System.Math.Abs(index);
            for (int i = 0; i < index; i++)
                action();
        }

        public static void ForEach(this int index, Action<int> action)
        {
            index = System.Math.Abs(index);
            for (int i = 0; i < index; i++)
                action(i);
        }

        public static void ForEachEnd<T>(this IEnumerable<T> enumerable, Action<T> itemAction)
        {
            foreach (var item in enumerable)
            {
                if (item != null)
                    itemAction(item);
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> itemAction)
        {
            foreach (var item in enumerable)
            {
                if (item != null)
                    itemAction(item);
            }

            return enumerable;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> itemAction)
        {
            if (enumerable is null)
                return enumerable;

            int i = 0;
            foreach (var item in enumerable)
            {
                if (item != null)
                    itemAction(item, i);

                i++;
            }

            return enumerable;
        }

        public static void ForEachN<T>(
            this IEnumerable<T> enumerable, int stepStart, int step, Action<T> onStep, Action<T> onOther = null)
        {
            if (step < 1)
                return;

            var array = enumerable.ToArray();
            int count = array.Length;

            for (int i = 0; i < stepStart; i++)
                onOther(array[i]);

            for (int i = stepStart; i < count; i += step)
            {
                onStep(array[i]);
                for (int j = i + 1; j < i + step && j < count; j++)
                    onOther(array[j]);
            }
        }

        public static IEnumerable<T> ForEachReversed<T>(this IEnumerable<T> enumerable, Action<T> itemAction)
        {
            var list = enumerable.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                if (item != null)
                    itemAction(item);
            }

            return enumerable;
        }

        public static async Task<IEnumerable<T>> ForEachReversedAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> itemAction)
        {
            var list = enumerable.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                if (item != null)
                    await itemAction(item);
            }

            return enumerable;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.IsNull() || enumerable.IsEmpty();
        }

        public static bool IsNullOrEmptyOrNotOne<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.IsNullOrEmpty() || !enumerable.IsOneCount();
        }

        public static bool IsNullOrEmptyOrOne<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.IsNullOrEmpty() || enumerable.IsOneCount();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random randomRange = null)
        {
            randomRange = randomRange ?? new Random();

            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = randomRange.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static T FirstBefore<T>(this IEnumerable<T> source, int index)
        {
            var array = source.ToArray();
            if (array.Length <= 1)
                return default;

            if (index >= array.Length)
                return array.FirstBeforeLast();

            return array[index - 1];
        }

        public static T FirstBeforeLast<T>(this IEnumerable<T> source)
        {
            var array = source.ToArray();
            if (array.Length <= 1)
                return default;

            return array[array.Length - 2];
        }

        public static T FirstBeforeLastOrFirst<T>(this IEnumerable<T> source)
        {
            if (source.Count() == 0)
                return default;

            if (source.Count() < 2)
                return source.First();

            return source.FirstBeforeLast();
        }

        public static IEnumerable<T> TakeExceptLast<T>(this IEnumerable<T> source) => source.Take(source.Count() - 1);
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item) => source.Except(new T[] { item });

        public static bool TryGetValue<T>(this IList<T> source, int index, out T value)
        {
            value = default;
            if (index < 0)
                return false;

            int count = source.Count();
            if (index >= count)
                return false;

            value = source.ToArray()[index];
            return true;
        }

        public static IEnumerable<Tuple<T1, T2>> ZipToTuples<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2) =>
            e1.Zip(e2, (x, y) => new Tuple<T1, T2>(x, y));

        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> source, Func<T, string> keySelector) =>
            source.GroupBy(i => keySelector(i)).Select(g => g.First());

        public static T AggregateOrDefault<T>(this IEnumerable<T> source, Func<T, T, T> aggregator)
        {
            if (source.IsNullOrEmpty())
                return default;

            if (source.Count() == 1)
                return source.First();

            return source.Aggregate(aggregator);
        }

        public static T AggregateOrEmpty<T>(this IEnumerable<T> source, Func<T, T, T> aggregator, T empty)
        {
            if (source.IsNullOrEmpty())
                return empty;

            return source.AggregateOrDefault(aggregator);
        }

        public static IEnumerable<T> AppendIfNotEmpty<T>(this IEnumerable<T> source, T item)
        {
            if (source.IsNullOrEmpty())
                return source;

            return source.Append(item);
        }

        public static IEnumerable<TSource>? WhereOrDefault<TSource>(
            this IEnumerable<TSource>? source, Func<TSource, bool> predicate) =>
            source is null ? null : source.Where(predicate);

        public static IEnumerable<TResult>? SelectOrDefault<TSource, TResult>(
            this IEnumerable<TSource>? source, Func<TSource, TResult> selector) =>
            source is null ? null : source.Select(selector);

        public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source.IsNullOrEmpty())
                return Enumerable.Empty<TResult>();

            return source.Select(selector).Where(i => i is not null);
        }

        public static TResult SelectValue<TSource, TResult>(this TSource source, Func<TSource, TResult> selector) =>
            selector(source);

        public static IEnumerable<T> ConcatOrEmpty<T>(this IEnumerable<T>? source, IEnumerable<T>? another)
        {
            if (source is null && another is null)
                return Array.Empty<T>();

            if (source is null)
                return another;

            if (another is null)
                return source;

            return source.Concat(another);
        }

        public static T[] ToArrayOrEmpty<T>(this IEnumerable<T>? source) =>
            source is null ? Array.Empty<T>() : source.ToArray();

        public static T[]? ToArrayOrDefault<T>(this IEnumerable<T>? source) =>
            source is null ? null : source.ToArray();

        public static List<T> ToListOrEmpty<T>(this IEnumerable<T>? source) =>
            source is null ? new() : source.ToList();

        public static List<T>? ToListOrDefault<T>(this IEnumerable<T>? source) =>
           source is null ? null : source.ToList();

        public static IEnumerable<TResult> SelectOrEmpty<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source.IsNullOrEmpty())
                return Enumerable.Empty<TResult>();

            return source.Select(selector);
        }

        public static IEnumerable<TSource> InsertOrEmpty<TSource>(this IEnumerable<TSource> source, int index, TSource item) =>
            source is null ? 
                Array.Empty<TSource>() : 
                source
                    .Take(index)
                    .Append(item)
                    .Concat(source.Skip(index));

        public static void SetToNull<T1, T2>(ref T1 v1, ref T2 v2)
            where T1 : class
            where T2 : class
        {
            v1 = null;
            v2 = null;
        }

        public static T[] SingleToArray<T>(this T @object)
        {
            if (@object is null)
                return Array.Empty<T>();

            return new T[] { @object };
        }

        public static List<T> CastToList<T>(this System.Collections.IEnumerable source) => source.Cast<T>().ToList();
        public static List<T> OfTypeToList<T>(this System.Collections.IEnumerable source) => source.OfType<T>().ToList();

        public static IEnumerable<Tuple<T1, IEnumerable<T2>>> Zip<T1, T2>(
            this IEnumerable<T1> src1,
            IEnumerable<T2> src2,
            Func<T1, T2, bool> check)
        {
            var list = new List<T2>();
            using (var enumerator = src2.GetEnumerator())
            {
                foreach (var item1 in src1)
                {
                    while (enumerator.MoveNext())
                    {
                        var pickedItem = enumerator.Current;
                        if (check(item1, pickedItem))
                        {
                            list.Add(pickedItem);
                        }
                        else
                        {
                            break;
                        }
                    }
                    var items = list.ToArray();
                    list.Clear();
                    yield return new Tuple<T1, IEnumerable<T2>>(item1, items);
                }
            }
        }

        public static IEnumerable<TResult> Concat<TResult>(params IEnumerable<object>[] other) =>
            other.AggregateOrDefault((x, y) => x.Concat(y)).Cast<TResult>();

        public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> selector) =>
            source.FirstOrDefault(e => selector(e)) != null;

        public static bool Contains<T>(this IEnumerable<object> source) =>
            source.OfType<T>().Any();

        public static T? FirstOrDefault<T>(this IEnumerable<T> source, Func<T, int, bool> selector)
        {
            int i = 0;
            return source.FirstOrDefault(item =>
            {
                if (selector(item, i))
                    return true;

                i++;

                return false;
            });
        }

        public static bool HasSameElements<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else
                    cnt.Add(s, 1);

            foreach (T s in list2)
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else
                    return false;
            
            return cnt.Values.All(c => c == 0);
        }
    }
}
