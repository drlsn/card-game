using System.Linq;
using System.Reflection;

namespace Corelibs.Basic.Reflection
{
    public static class TypeGetMethodExtensions
    {
        public static MethodBase GetMethod(this object[] @objects, string name)
        {
            foreach (var obj in objects)
            {
                var objectType = obj.GetType();
                var methods = objectType.GetMethods();
                var methodsOfName = methods.Where(m => m.Name == name);
                if (methodsOfName.Any())
                    return methodsOfName.First();
            }

            return null;
        }

        public static MethodBase GetMethodWithParamType<TObject, TParam>(TObject[] @objects, out object @object)
        {
            @object = null;
            foreach (var obj in objects)
            {
                var method = GetMethodWithParamType<TObject, TParam>(obj);
                if (method != null)
                {
                    @object = obj;
                    return method;
                }
            }

            return null;
        }

        public static MethodBase GetMethodWithParamType<TObject, TParam>(TObject @object)
        {
            var objectType = @object.GetType();
            var paramType = typeof(TParam);
            var methods = objectType.GetMethods();
            var method = methods
                .Where(m => m
                    .GetParameters()
                    .FirstOrDefault(p => p.ParameterType == paramType) != null)
                .FirstOrDefault();

            return method;
        }
    }
}
