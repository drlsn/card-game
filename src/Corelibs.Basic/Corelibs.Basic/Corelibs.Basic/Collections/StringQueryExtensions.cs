using System.Reflection;
using System.Web;

namespace Corelibs.Basic.Collections
{
    public static class StringQueryExtensions
    {
        public static string GetQueryString(this object obj)
        {
            var propertiesAll = obj.GetType().GetProperties().Where(p => p.GetValue(obj) != null).ToArray();
            var queryProperties = propertiesAll.Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj).ToString()));

            return String.Join("&", queryProperties.ToArray());
        }

        public static string GetQueryString(this object obj, Type routeAttributeType)
        {
            var propertiesAll = obj.GetType().GetProperties().Where(p => p.GetValue(obj) != null).ToArray();
            var propertiesForQuery = propertiesAll.Where(p => p.GetCustomAttribute(routeAttributeType) == null).ToArray();
            var queryProperties = propertiesForQuery.Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj).ToString()));

            return string.Join("&", queryProperties.ToArray());
        }
    }
}
