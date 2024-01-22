using System.Collections.Generic;
using System.Linq;

namespace Corelibs.Basic.Collections
{
    public static class EnumerableCountExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Count() == 0;
        }

        public static bool IsOneCount<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Count() == 1;
        }

        public static bool IsIndexInRange<T>(this IEnumerable<T> enumerable, int index)
        {
            return index >= 0 && index < enumerable.Count();
        }
    }
}
