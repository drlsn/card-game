using System.Reflection;

namespace Corelibs.Basic.Reflection
{
    public static class AssemblyExtensionsEx
    {
        public static Type[] GetCurrentAssemblyTypesDerivedFrom<T>(this Assembly assembly)
            where T : class
        {
            var types = assembly.GetTypes();
            var classTypes = types.Where(t => t.IsClass).ToArray();
            var assignableFromTTypes = classTypes.Where(t => typeof(T).IsAssignableFrom(t)).ToArray();

            return assignableFromTTypes;
        }

        public static Type[] GetCurrentDomainTypesImplementing<T>(Assembly assembly = null)
        {
            var assemblies = assembly is null ? AppDomain.CurrentDomain.GetAssemblies() : new[] { assembly };

            var type = typeof(T);
            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Where(p => !p.IsAbstract && !p.IsInterface)
                .ToArray();

            return types;
        }

        public static Type[] GetTypesInFolder(this Assembly assembly, string folder)
        {
            string rootNamespace = assembly.GetName().Name + "." + folder.Replace("/", ".");

            Type[] allTypes = assembly.GetTypes();

            return allTypes.Where(type => 
                type.Namespace != null && 
                type.Namespace.StartsWith(rootNamespace) &&
                !type.IsNested).ToArray();
        }
    }
}
