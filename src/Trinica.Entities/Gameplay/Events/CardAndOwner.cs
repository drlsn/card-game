using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public record CardAndOwner(ICard Card, UserId PlayerId);
