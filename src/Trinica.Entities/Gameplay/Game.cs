using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>
{
    public Player[] Players { get; private set; }
    public FieldDeck CommonPool { get; private set; }
    public ICard CenterCard { get; private set; }
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

    // TO DO: by amount and source (common pool or own)
    public void TakeCardsToHand(Random random)
    {
        Players.TakeCardsToHand(random);
    }

    public void CalculateRoundPlayerOrder()
    {
        // TO DO: order by speed of heroes only?
        MoveOrder = Players
            .GetPlayersOrder()
            .ToIds();
    }

    public void LayCardsToBattle(UserId playerId, CardToLay[] cards)
    {
        var player = Players.OfId(playerId);
        if (CenterCard is not null)
        {
            var cardToCenter = cards.FirstOrDefault(c => c.ToCenter);
            if (cardToCenter is not null)
            {
                CenterCard = player.TakeCardFromHand(cardToCenter.SourceCardId);
                cards = cards.Except(cardToCenter).ToArray();
            }
        }

        player.LayCardsToBattle(cards);
    }

    public void PlayDices(UserId playerId, int n, Func<Random> getRandom)
    {
        var player = Players.OfId(playerId);
        player.PlayDices(n, getRandom);
    }

    public void AssignDicesToCards(UserId playerId, DiceOutcomeIndexPerCard[] assigns)
    {
        var player = Players.OfId(playerId);
        player.AssignDicesToCards(assigns);
    }

    public void AssignCardsTargets(UserId playerId, CardTarget[] targets)
    {
        var player = Players.OfId(playerId);
        player.AssignDicesToCards(assigns);
    }

    public void PerformRound()
    {
        MoveOrder.ForEach(playerId =>
        {
            var player = Players.OfId(playerId);
            player.DiceOutcomesPerCard.ForEach(outcomePerCard =>
            {
                var card = player.GetBattlingCard(outcomePerCard.CardId);
            });
        });
    }
}
