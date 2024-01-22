using Corelibs.Basic.Collections;

namespace Corelibs.Basic.Collections
{
    public static class GuidExtensions
    {
        public static bool IsIDs(this IEnumerable<string> ids)
        {
            if (ids.IsNullOrEmpty())
                return false;

            return ids.All(IsID);
        }

        public static bool IsID(this string id)
        {
            if (id.IsNullOrEmpty())
                return false;

            return Guid.TryParse(id, out var guid);
        }
    }
}
