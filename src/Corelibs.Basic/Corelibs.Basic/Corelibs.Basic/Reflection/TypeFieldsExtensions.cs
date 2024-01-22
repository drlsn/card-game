using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Corelibs.Basic.Reflection
{
    public static class TypeFieldsExtensions
    {
        public static IEnumerable<FieldInfo> GetThisAndBaseFields(this object @object, BindingFlags bindingFlags)
        {
            var result = new Dictionary<string, FieldInfo>();

            Type currentType = @object.GetType();
            while (currentType != null)
            {
                var fields = currentType.GetFields(bindingFlags);
                foreach (var field in fields)
                    if (!result.TryGetValue(field.Name, out var typename))
                        result.Add(field.Name, field);

                currentType = currentType.BaseType;
            }

            return result.Values;
        }

        public static IEnumerable<FieldInfo> GetPublicFields<T>(this object @object)
        {
            return @object
                .GetType()
                .GetFields()
                .Where(f => f.IsPublic);
        }

        public static FieldInfo GetPublicField<T>(this object @object)
        {
            return @object.GetPublicFields<T>().FirstOrDefault();
        }

        public static IEnumerable<FieldInfo> GetFieldsOfType<T>(this IEnumerable<FieldInfo> fields)
        {
            var desiredType = typeof(T);

            var result = new List<FieldInfo>();
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (fieldType == desiredType)
                {
                    result.Add(field);
                    continue;
                }

                if (fieldType.IsSubclassOf(desiredType))
                {
                    result.Add(field);
                    continue;
                }

                if (desiredType.IsAssignableFrom(fieldType))
                {
                    result.Add(field);
                    continue;
                }
            }

            return result;
        }

        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<TFieldType, TAttType>(this object @object)
        {
            var fields = @object.GetThisAndBaseFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fieldsOfType = fields.GetFieldsOfType<TFieldType>();
            var fieldsWithAtt = fieldsOfType.Where(f => f.IsDefined(typeof(TAttType), false));

            return fieldsWithAtt;
        }

        public static FieldInfo GetFieldWithAttribute<TFieldType, TAttType>(this object @object)
        {
            return @object.GetFieldsWithAttribute<TFieldType, TAttType>().FirstOrDefault();
        }

        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttType>(this object @object)
        {
            var fields = @object.GetThisAndBaseFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fieldsWithAtt = fields.Where(f => f.IsDefined(typeof(TAttType), false));

            return fieldsWithAtt;
        }
    }
}
