using Trinica.Entities.Users;
using Corelibs.Basic.DDD;
using System.Linq;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>
{
    public Player[] Players { get; private set; }
    public FieldDeck CommonPool { get; private set; }
    public CardId CenterCard { get; private set; }
    public UserId[] MoveOrder { get; private set; }

    public Game(
        GameId id,
        Player[] players) : base(id)
    {
        Players = players;
    }

    public void TakeCardsToCommonPool(Random random)
    {
        CommonPool = Players.ShuffleAllAndTakeHalfCards(random);
    }

    public void TakeCardsToHand(Random random)
    {
        Players.TakeCardsToHand(random);
    }

    public void CalculateRoundPlayerOrder()
    {
        MoveOrder = Players
            .GetPlayersOrder()
            .Select(p => p.Id)
            .ToArray();
    }

    public void LayCardsToBattle(UserId playerId, CardToLay[] cards)
    {
        Players
            .FirstOrDefault(p => p.Id == playerId)?
            .LayCardsToBattle(cards);
    }
}
