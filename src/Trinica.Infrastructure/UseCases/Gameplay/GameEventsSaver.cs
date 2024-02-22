using Corelibs.Basic.Events;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class GameEventsSaver(
    IEventStore<GameId, GameEvent> eventStore) :
    INotificationHandler<GameStartedEvent>,
    INotificationHandler<GameFinishedEvent>,
    INotificationHandler<CardsTakenToHandEvent>,
    INotificationHandler<LayCardDownOrderCalculatedEvent>,
    INotificationHandler<CardsLaidDownEvent>,
    INotificationHandler<CardsLayPassedByPlayerEvent>,
    INotificationHandler<DicesPlayedEvent>,
    INotificationHandler<DicesReplayPassedEvent>,
    INotificationHandler<AssignsDicesToCardConfirmedEvent>,
    INotificationHandler<AssignTargetToCardEvent>,
    INotificationHandler<AssignTargetsToCardConfirmedEvent>
{
    private readonly IEventStore<GameId, GameEvent> _eventStore = eventStore;

    public ValueTask Handle(GameStartedEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(GameFinishedEvent ev, CancellationToken ct) => Save(ev, ct);

    public ValueTask Handle(CardsTakenToHandEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(LayCardDownOrderCalculatedEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(CardsLaidDownEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(CardsLayPassedByPlayerEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(DicesPlayedEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(DicesReplayPassedEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(AssignsDicesToCardConfirmedEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(AssignTargetToCardEvent ev, CancellationToken ct) => Save(ev, ct);
    public ValueTask Handle(AssignTargetsToCardConfirmedEvent ev, CancellationToken ct) => Save(ev, ct);

    private async ValueTask Save(GameEvent ev, CancellationToken ct) =>
        await _eventStore.Save(ev.GameId, ev);
}
