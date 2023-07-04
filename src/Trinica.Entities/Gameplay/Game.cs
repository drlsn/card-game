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
    public UserId[] CardsLayOrderPerPlayer { get; private set; }

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

    public void TakeCardsToHand(UserId playerId, CardToTake[] cards, Random random)
    {
        var player = Players.OfId(playerId);
        cards.ForEach(card =>
        {
            if (card.Source == CardSource.CommonPool)
                player.AddCardToHand(CommonPool.TakeCard(random));
            else
            if (card.Source == CardSource.Own)
                player.TakeCardToHand(random);
        });
    }

    public void CalculateLayDownOrderPerPlayer()
    {
        CardsLayOrderPerPlayer = Players
            .GetPlayersOrderedByHeroSpeed()
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
        player.AssignCardsTargets(targets);
    }

    public void PerformRound()
    {
        CardsLayOrderPerPlayer.ForEach(playerId =>
        {
            var player = Players.OfId(playerId);
            player.DiceOutcomesPerCard.ForEach(outcomePerCard =>
            {
                var card = player.GetBattlingCard(outcomePerCard.SourceCardId);
            });
        });
    }
}
