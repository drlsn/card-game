namespace Corelibs.Basic.Repository;

public interface IAccessor<out T>
{
    T Get();
}

public interface IAccessorAsync<T>
{
    Task<T> Get();
}

public interface IAccessor<out T1, out T2>
{
    T1 Get();
}

public class SimpleAccessor<T> : IAccessor<T>
{
    private readonly T _service;

    public SimpleAccessor(T service) => _service = service;

    T IAccessor<T>.Get() => _service;
}

public class SimpleAccessor<T1, T2> : SimpleAccessor<T1>
{
    public SimpleAccessor(T1 service) : base(service) {}
}
