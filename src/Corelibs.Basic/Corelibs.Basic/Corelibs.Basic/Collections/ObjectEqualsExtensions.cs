namespace Corelibs.Basic.Collections
{
    public static class ObjectEqualsExtensions
    {
        public static bool Equals(params object[] objects)
        {
            if (objects.IsNullOrEmpty())
                return true;

            if (objects.Length == 1)
                return true;

            foreach (var obj in objects)
                if (objects[0] != obj)
                    return false;

            return true;
        }
    }
}
