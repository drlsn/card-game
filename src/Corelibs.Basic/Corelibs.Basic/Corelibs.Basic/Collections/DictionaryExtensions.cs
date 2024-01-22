namespace Corelibs.Basic.Collections;

public static class DictionaryExtensions
{
    public static TValue TryGetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        where TValue : new()
    {
        if (!source.TryGetValue(key, out var value))
        {
            value = new TValue();
            source.Add(key, value);
        }

        return value;
    }

    public static void AddToListValue<TKey, TValue>(this IDictionary<TKey, List<TValue>> source, TKey key, TValue value)
    {
        var listValue = source.TryGetOrAddValue(key);
        listValue.Add(value);
    }
}
