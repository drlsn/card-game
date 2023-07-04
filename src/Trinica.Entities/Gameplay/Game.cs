using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Trinica.Entities.Gameplay.Cards;
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

    public void AssignDiceToCard(UserId playerId, int diceIndex, CardId cardId)
    {
        var player = Players.OfId(playerId);
        player.AssignDiceToCard(diceIndex, cardId);
    }

    public void ChooseCardSkill(UserId playerId, CardId cardId, int skillIndex)
    {
        var player = Players.OfId(playerId);
        player.ChooseCardSkill(cardId, skillIndex);
    }

    public void AssignCardsTargets(UserId playerId, CardId cardId, CardId targetCardId)
    {
        var player = Players.OfId(playerId);
        player.AssignCardsTargets(cardId, targetCardId);
    }

    public void PerformRound(Random random)
    {
        var cards = Players.GetBattlingCardsBySpeed(random);

        cards.ForEach(card =>
        {
            var player = Players.GetPlayerWithCard(card.Id);
            var cardAssignments = player.CardAssignments[card.Id];
            var targetCard = cards.First(c => c.Id == cardAssignments.TargetCardId);

            if (card is SpellCard spellCard)
            {

            }
            else
            if (card is UnitCard unitCard)
            {
                if (cardAssignments.DiceOutcome == DiceOutcome.Attack)
                    ;
                else
                    ;
            }
            else
            if (card is HeroCard heroCard)
            {
                if (cardAssignments.DiceOutcome == DiceOutcome.Attack)
                    ;
                else
                    ;
            }
        });
    }
}
