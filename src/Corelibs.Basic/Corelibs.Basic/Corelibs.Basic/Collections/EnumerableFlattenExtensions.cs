namespace Corelibs.Basic.Collections
{
    public static class EnumerableFlattenExtensions
    {
        public static IEnumerable<T> Flatten<T>(
           this T source,
           Func<T, IEnumerable<T>> getChildrenFunction)
        {
            var initialChildren = getChildrenFunction(source);
            return initialChildren.Flatten(getChildrenFunction, 0);
        }

        public static IEnumerable<T> Flatten<T>(
            this T source,
            Func<T, IEnumerable<T>> getChildrenFunction,
            int recursionCountLimit)
        {
            var initialChildren = getChildrenFunction(source);
            return initialChildren.Flatten(getChildrenFunction, recursionCountLimit);
        }

        public static IEnumerable<T> Flatten<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getChildrenFunction)
        {
            return source.Flatten(getChildrenFunction, out int recursionCount);
        }

        public static IEnumerable<T> Flatten<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getChildrenFunction,
            out int recursionCount,
            int recursionCountLimit = 0)
        {
            recursionCount = 0;
            return source.FlattenAndGetRecursionCountByRef(getChildrenFunction, ref recursionCount, recursionCountLimit);
        }

        public static IEnumerable<T> Flatten<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getChildrenFunction,
            int recursionCountLimit)
        {
            return source.Flatten(getChildrenFunction, out int recursionCount, recursionCountLimit);
        }

        /// <summary>
        /// Flatten nested enumerable tree to single array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getChildrenFunction"></param>
        /// <param name="recursionCountLimit">If no limit, then should be set to 0 or less.</param>
        /// <param name="recursionCount"></param>
        /// <returns></returns>
        private static IEnumerable<T> FlattenAndGetRecursionCountByRef<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getChildrenFunction,
            ref int recursionCount,
            int recursionCountLimit = 0)
        {
            if (source == null)
                return null;

            if (recursionCountLimit > 0 && recursionCount >= recursionCountLimit)
                return source;

            bool areThereChildren = source.Any(element => getChildrenFunction(element).Count() != 0);
            if (areThereChildren)
                recursionCount++;

            foreach (T element in source)
            {
                var children = getChildrenFunction(element);
                if (children.IsNullOrEmpty())
                    continue;

                var childrenFlattened = children.FlattenAndGetRecursionCountByRef(getChildrenFunction, ref recursionCount, recursionCountLimit);
                source = source.Concat(childrenFlattened);
            }

            return source;
        }
    }
}
