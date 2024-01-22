using System.Collections;

namespace Corelibs.Basic.Collections;

public interface IRemoveOnlyList<out T> : IEnumerable<T>
{
    bool RemoveAt(int index);
    T Take(Func<T, bool> selector);
    T[] Take(int count);
    int Count { get; }
    T this[int index] { get; }
}

public class RemoveOnlyList<T> : IRemoveOnlyList<T>
{
    private T[] _source;

    public int Count { get; private set; }

    public RemoveOnlyList(IEnumerable<T> source)
    {
        _source = source.ToArray();
        Count = _source.Length;
    }

    public T this[int index] {
        get {
            if ((uint) index >= (uint) Count)
                throw new IndexOutOfRangeException();

            return _source[index];
        }

        set {
            if ((uint) index >= (uint) Count)
                throw new IndexOutOfRangeException();

            _source[index] = value;
        }
    }

    /// <summary>
    /// Removes an item at specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>True if successfully removed an element, false otherwise.</returns>
    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            return false;

        _source.Swap(index, Count - 1);
        _source[Count - 1] = default;
        --Count;

        return true;
    }

    public T Take(Func<T, bool> selector)
    {
        var i = Array.FindIndex(_source, t => selector(t));
        var item = _source[i];
        if (!RemoveAt(i))
            return default;

        return item;
    }

    public T[] Take(int count)
    {
        var taken = _source.Take(count).ToArray();

        _source = _source.Skip(count).Where(v => v is not null).ToArray();
        Count = _source.Count();

        return taken;
    }

    IEnumerator IEnumerable.GetEnumerator() => _source.Take(Count).GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in _source.Take(Count))
            yield return item;
    }
}

public static class RemoveOnlyListExtensions
{
    public static IRemoveOnlyList<T> ToRemoveOnlyList<T>(this IEnumerable<T> source) => 
        new RemoveOnlyList<T>(source);
}
