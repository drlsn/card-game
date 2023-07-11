using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class CardsTakenToHandEvent : INotification
{
    public UserId PlayerId { get; }
    public GameId GameId { get; }
}
