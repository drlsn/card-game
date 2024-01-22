namespace Corelibs.Basic.DDD;

public interface IDomainEvent
{
    string Id { get; }
    long Timestamp { get; }
}
