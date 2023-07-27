using Mediator;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class DamageInflictedEvent : GameEvent, INotification
{
    public DamageInflictedEvent(
        GameId gameId,
        PlayerData attacker,
        PlayerData target,
        int damage) : base(gameId, attacker.PlayerId)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
    }

    public PlayerData Attacker { get; }
    public PlayerData Target { get; }
    public int Damage { get; }

    public override string ToMessage() =>
        $"{Attacker.CardName} inflicts {Damage} damage to {Target.CardName}";

    public record PlayerData(
        UserId PlayerId,
        string PlayerName,
        CardId CardId,
        string CardName);
}
