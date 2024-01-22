namespace Corelibs.Basic.Functional;

public static class MatchExtensions
{
    public static TResult Match<TSource, TResult>(this TSource source, params (TSource, TResult)[] selectors)
        where TSource : class =>
        selectors.FirstOrDefault(match => match.Item1 == source).Item2;
}
