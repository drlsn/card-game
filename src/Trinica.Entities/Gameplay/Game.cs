using CardGame.Entities.Users;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>
{
    public Player[] Players { get; private set; }
    public FieldDeck CommonPool { get; private set; }

    public UserId[] CurrentRoundMoveOrder { get; private set; }

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
        Players.TakeNCardsToHand(random);
    }

    public void CalculateRoundPlayerOrder()
    {
        CurrentRoundMoveOrder = Players
            .GetPlayersOrder()
            .Select(p => p.Id)
            .ToArray();
    }
}
