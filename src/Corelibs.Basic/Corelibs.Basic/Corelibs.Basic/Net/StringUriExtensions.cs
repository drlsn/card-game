using Corelibs.Basic.Collections;
using Corelibs.Basic.Reflection;

namespace Corelibs.Basic.Net
{
    public static class StringUriExtensions
    {
        public static string ReplaceParametersWithValues(this string uri, object @object)
        {
            var uriLower = uri.ToLower();

            uriLower.Substrings('{', '}', out var routeParameterNames);
            if (routeParameterNames.Length == 0)
                return uri;

            var properties = @object.GetType().GetProperties(routeParameterNames, caseSensitive: false);

            foreach (var property in properties)
            {
                var value = property.GetValue(@object) as string;
                if (value.IsNullOrEmpty())
                    continue;

                var propertyNameLower = property.Name.ToLower();
                uri = uriLower.Replace(propertyNameLower, value);
            }

            return uri.Remove("{", "}");
        }
    }
}
