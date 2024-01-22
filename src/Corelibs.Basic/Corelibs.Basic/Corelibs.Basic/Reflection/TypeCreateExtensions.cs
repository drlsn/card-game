namespace Corelibs.Basic.Reflection;

public static class TypeCreateExtensions
{
    public static Type CreateInterface(Type genericInterfaceType, params Type[] typeArguments)
    {
        if (!genericInterfaceType.IsGenericType || !genericInterfaceType.IsInterface)
            throw new ArgumentException("The provided type is not a generic interface.");

        if (typeArguments.Length != genericInterfaceType.GetGenericArguments().Length)
            throw new ArgumentException("The number of type arguments does not match the generic interface.");

        return genericInterfaceType.MakeGenericType(typeArguments);
    }

    public static Type CreateClass(Type genericClassType, params Type[] typeArguments)
    {
        if (!genericClassType.IsGenericType || genericClassType.IsInterface || genericClassType.IsAbstract)
            throw new ArgumentException("The provided type is not a generic class.");

        if (typeArguments.Length != genericClassType.GetGenericArguments().Length)
            throw new ArgumentException("The number of type arguments does not match the generic class.");

        return genericClassType.MakeGenericType(typeArguments);
    }
}
