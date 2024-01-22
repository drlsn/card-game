using Corelibs.Basic.Collections;
using System.Reflection;

namespace Corelibs.Basic.Reflection
{
    public static class TypePropertiesExtensions
    {
        public static PropertyInfo[] GetProperties(this Type type, IList<string> names, bool caseSensitive = true)
        {
            if (!caseSensitive)
                names = names.ToLower();

            return type.GetProperties().Where(p => names.Contains(caseSensitive ? p.Name : p.Name.ToLower())).ToArray();
        }

        public static PropertyInfo GetProperty(this Type type, string name, bool caseSensitive)
        {
            if (caseSensitive)
                return type.GetProperty(name);

            return type.GetProperties(new[] { name }, caseSensitive).FirstOrDefault();
        }

        public static T GetPropertyValue<T>(this object @object)
        {
            var objectType = @object.GetType();
            var propertyType = typeof(T);

            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var property = properties.FirstOrDefault(p => p.PropertyType == propertyType);
            if (property == null)
                return default;

            var value = property.GetValue(@object);
            if (value is T tValue)
                return tValue;

            return default;
        }
    }
}
