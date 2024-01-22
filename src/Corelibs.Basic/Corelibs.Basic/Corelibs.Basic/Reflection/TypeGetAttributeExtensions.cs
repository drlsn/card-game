namespace Corelibs.Basic.Reflection
{
    public static class TypeGetAttributeExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
                return null;

            var field = type.GetField(name);
            if (field == null)
                return null;

            var attr = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
            if (attr == null)
                return null;

            return attr;
        }
    }
}
