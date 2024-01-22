namespace Corelibs.Basic.Collections;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static string WhitespaceIfNullOrEmpty(this string str)
    {
        if (str.IsNullOrEmpty())
            return " ";

        return str;
    }

    public static bool Substring(this string str, char firstCharacter, char secondCharacter, out string result) =>
        str.Substring(firstCharacter, secondCharacter, out result, out int firstIndex, out int secondIndex);

    public static bool Substring(
        this string str, char firstCharacter, char secondCharacter, out string result, out int firstIndex, out int secondIndex)
    {
        result = null;
        firstIndex = -1;
        secondIndex = -1;

        if (str.IsNullOrEmpty())
            return false;

        firstIndex = str.IndexOf(firstCharacter);
        secondIndex = str.IndexOf(secondCharacter);
        if (firstIndex < 0 || secondIndex < 0)
            return false;

        if (firstIndex >= secondIndex)
            return false;

        result = str.Substring(firstIndex + 1, secondIndex - firstIndex - 1);
        if (result.IsNullOrEmpty())
            return false;

        return true;
    }

    public static bool Substrings(
        this string str, char firstCharacter, char secondCharacter, out string[] result)
    {
        var list = new List<string>();

        const int iCountLimit = 1000;
        int i = 0;
        while (i < iCountLimit)
        {
            if (!str.Substring(firstCharacter, secondCharacter, out string singleResult, out int firstIndex, out int secondIndex))
                break;

            str = str.Remove(0, secondIndex);
            list.Add(singleResult);
            i++;
        }

        result = list.ToArray();

        return !list.IsEmpty();
    }

    public static bool SubstringAfter(this string str, char character, out string result)
    {
        result = null;
        int i = str.IndexOf(character);
        if (i < 0)
            return false;

        result = str.Substring(i + 1);
        if (str.IsNullOrEmpty())
            return false;

        return true;
    }

    public static bool SubstringBefore(this string str, char character, out string result)
    {
        result = null;
        int i = str.IndexOf(character);
        if (i < 0)
            return false;

        result = str.Substring(0, i);
        if (str.IsNullOrEmpty())
            return false;

        return true;
    }

    public static string SubstringBeforeOrThis(this string str, char character)
    {
        if (str.SubstringBefore(character, out var result))
            return result;

        return str;
    }

    public static string[] ToLower(this IEnumerable<string> str) =>
        str.ForEach(s => s.ToLower()).ToArray();

    public static string[] ToUpper(this IEnumerable<string> str) =>
       str.ForEach(s => s.ToUpper()).ToArray();

    public static string Remove(this string str, string ocurrence) =>
       str.Replace(ocurrence, "");

    public static string Remove(this string str, params string[] ocurrences)
    {
        foreach (var ocurrence in ocurrences)
            str = str.Remove(ocurrence);

        return str;
    }

    public static string ToUpperFirstLetter(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        
        char[] letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        
        return new string(letters);
    }
}
