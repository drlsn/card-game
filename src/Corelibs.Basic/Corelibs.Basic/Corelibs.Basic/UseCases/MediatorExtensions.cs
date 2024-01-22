using Corelibs.Basic.DDD;
using Mediator;

namespace Corelibs.Basic.UseCases;

public static class MediatorExtensions
{
    public static Task PublishEvents(
        this IPublisher publisher,
        IEntity entity)
    {
        var events = entity.DomainEvents.ToList();
        entity.DomainEvents.Clear();
        
        return Task.WhenAll(events.Select(async n => await publisher.Publish(n)).ToArray());
    }
}
