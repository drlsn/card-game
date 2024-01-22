namespace Corelibs.Basic.Functional
{
    public static class NullableExtensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool HasAnyValue(params object?[] objects)
        {
            foreach (var @object in objects)
                if (@object != null)
                    return true;

            return false;
        }
    }
}
