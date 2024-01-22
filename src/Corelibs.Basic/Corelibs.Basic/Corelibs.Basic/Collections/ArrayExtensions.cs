namespace Corelibs.Basic.Collections;

public static class ArrayExtensions
{
    public static T[] CreateArray<T>(params IEnumerable<T>[] arrays)
    {
        arrays = arrays.Where(arr => !arr.IsNullOrEmpty()).ToArray();

        int totalLength = arrays.Sum(arr => arr.Count());
        var result = new T[totalLength];

        int currentIndex = 0;
        foreach (var arr in arrays)
        {
            foreach (T item in arr)
            {
                result[currentIndex] = item;
                currentIndex++;
            }
        }

        return result;
    }
}
