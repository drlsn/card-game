using System.Collections.Generic;

namespace Corelibs.Basic.Collections
{
    public static class StringListExtensions
    {
        public static bool InsertOrAdd(this List<string> list, string id, string createAfterItemID = null)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (list.Contains(id))
                return false;

            if (string.IsNullOrEmpty(createAfterItemID))
            {
                list.Add(id);
                return true;
            }

            var index = list.IndexOf(createAfterItemID);
            if (index == -1)
                return false;

            list.Insert(index + 1, id);
            return true;
        }
    }
}
