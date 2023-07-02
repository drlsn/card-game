using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

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

    public void PlayDices(UserId playerId, int n, Func<Random> getRandom)
    {
        var player = Players.OfId(playerId);
        player.PlayDices(n, getRandom);
    }

    public void AssignDicesToCardsAndSetActionOrder(UserId playerId, DiceOptionIndexPerCard[] assigns)
    {
        var player = Players.OfId(playerId);
        player.AssignDicesToCards(assigns);
    }

    public void PerformRound()
    {
    }
}

