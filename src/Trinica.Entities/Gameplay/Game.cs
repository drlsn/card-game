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

    public RoundSettings RoundSettings { get; private set; }

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

    public void StartRound(Random random)
    {
        var cards = Players.GetBattlingCardsBySpeed(random);

        cards.ForEach(card =>
        {
            if (card is not ICombatCard combatCard)
                return;

            combatCard.Effects.ForEach(effect =>
                effect.OnRoundStart(combatCard, null, null, RoundSettings));
        });
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

    public void RemoveDiceFromCard(UserId playerId, CardId cardId)
    {
        var player = Players.OfId(playerId);
        player.RemoveDiceFromCard(cardId);
    }

    public void ChooseCardSkill(UserId playerId, CardId cardId, int skillIndex)
    {
        var player = Players.OfId(playerId);
        player.ChooseCardSkill(cardId, skillIndex);
    }

    public void AssignCardTarget(UserId playerId, CardId cardId, CardId targetCardId)
    {
        var player = Players.OfId(playerId);
        player.AssignCardTarget(cardId, targetCardId);
    }

    public void RemoveCardTarget(UserId playerId, CardId cardId, CardId targetCardId)
    {
        var player = Players.OfId(playerId);
        player.RemoveCardTarget(cardId, targetCardId);
    }

    public void PerformRound(Random random)
    {
        var cards = Players.GetBattlingCardsBySpeed(random);

        cards.ForEach(card =>
        {
            var player = Players.GetPlayerWithCard(card.Id);
            var cardAssignment = player.CardAssignments[card.Id];
            var targetCards = cards.Where(c => cardAssignment.TargetCardIds.Contains(c.Id)).Cast<ICombatCard>().ToArray();
            targetCards = targetCards.Where(c => !RoundSettings.NotAllowedAsTargetCards.ContainsKey(c.Id)).ToArray();

            var otherPlayers = Players.NotOfId(player.Id);
            var enemiesBattlingCards = otherPlayers.GetBattlingCards().OfType<ICombatCard>().ToArray();

            if (!RoundSettings.PrioritizedToAttackCards.IsNullOrEmpty())
            {
                var cardId = RoundSettings.PrioritizedToAttackCards.Values.Shuffle(random).First();
                targetCards = enemiesBattlingCards.Where(c => c.Id == cardId).ToArray();
            }

            if (card is not ICombatCard combatCard)
                return;

            var moveType = cardAssignment.DiceOutcome.IsElement() ? MoveType.Skill : MoveType.Attack;

            // ----- use items! -----
            var cardWithItems = combatCard as ICardWithItems;
            if (cardWithItems is not null)
                cardWithItems.ItemCards.ForEach(itemCard =>
                    combatCard.Statistics.Modify(itemCard.Statistics, itemCard.Id));

            // Attacker - BeforeMoveAtAll
            var moveAtAll = new Move()
            {
                Damage = CalculateDamage(combatCard, null, moveType),
                Type = moveType
            };
            combatCard.Effects.ForEach(effect =>
                effect.BeforeMoveAtAll(combatCard, new(targetCards, enemiesBattlingCards, null), moveAtAll));

            // Attacker - BeforeMoveAtSingleTarget
            var movesAtSingle = new Dictionary<CardId, Move>();
            combatCard.Effects.ForEach(effect =>
               targetCards.ForEach(targetCard =>
               {
                   var damage = CalculateDamage(combatCard, targetCard, moveType);
                   movesAtSingle.Add(targetCard.Id, new Move()
                   {
                       Damage = moveAtAll.Damage,
                       Type = moveType
                   });

                   effect.BeforeMoveAtSingleTarget(combatCard, targetCard, movesAtSingle[targetCard.Id]);
               }));

            // Defender - BeforeReceive
            targetCards.ForEach(targetCard =>
            {
                targetCard.Effects.ForEach(effect =>
                    effect.BeforeReceive(targetCard, new(combatCard, enemiesBattlingCards, null), movesAtSingle[targetCard.Id]));
            });

            // Update After Move Modified By Effects
            if (cardWithItems is not null && !moveAtAll.ItemsEnabled)
                cardWithItems.ItemCards.ForEach(itemCard =>
                    combatCard.Statistics.RemoveAll(itemCard.Id));

            // ------------------------
            // PERFORM ACTION!!!
            // ------------------------
            if (moveAtAll.MoveEnabled)
            {
                if (moveType is MoveType.Attack && moveAtAll.AttackEnabled)
                {
                    targetCards.ForEach(targetCard =>
                    {
                        var move = movesAtSingle[targetCard.Id];
                        targetCard.Statistics.HP.Modify(move.Damage);
                    });
                }
                else
                if (moveType is MoveType.Skill && moveAtAll.SkillsEnabled)
                {
                    targetCards.ForEach(targetCard =>
                    {
                        var move = movesAtSingle[targetCard.Id];
                        if (!move.SkillsEnabled)
                            return;

                        var doesPowerDamage = combatCard.DoesPowerDamage(cardAssignment.SkillIndex);
                        if (doesPowerDamage)
                            targetCard.Statistics.HP.Modify(move.Damage);

                        if (move.EffectsEnabled)
                        {
                            var effects = combatCard.GetEffects(cardAssignment.SkillIndex);
                            targetCard.Effects.AddRange(effects);
                        }
                    });
                }
            }

            // Attacker - AfterMoveAtAll
            combatCard.Effects.ForEach(effect =>
                effect.AfterMoveAtAll(combatCard, new(targetCards, enemiesBattlingCards, null), new()
                {
                    Damage = 23,
                    Type = moveType
                }));

            // Attacker - AfterMoveAtSingleTarget
            combatCard.Effects.ForEach(effect =>
               targetCards.ForEach(targetCard =>
               {
                   var damage = CalculateDamage(combatCard, targetCard, moveType);
                   effect.AfterMoveAtSingleTarget(combatCard, targetCard, new()
                   {
                       Damage = damage,
                       Type = moveType
                   });
               }));

            // Defender - AfterReceive
            targetCards.ForEach(targetCard =>
            {
                var damage = CalculateDamage(combatCard, targetCard, moveType);
                targetCard.Effects.ForEach(effect =>
                    effect.AfterReceive(targetCard, new(combatCard, enemiesBattlingCards, null), new()
                    {
                        Damage = 23,
                        Type = moveType
                    }));
            });

            // ----- unuse items! -----
            if (cardWithItems is not null)
                cardWithItems.ItemCards.ForEach(itemCard =>
                    combatCard.Statistics.RemoveAll(itemCard.Id));
        });
    }

    public void FinishRound(Random random)
    {
        var cards = Players.GetBattlingCardsBySpeed(random);

        cards.ForEach(card =>
        {
            if (card is not ICombatCard combatCard)
                return;

            combatCard.Effects.ForEach(effect =>
                effect.OnRoundFinish(combatCard));
        });
    }

    public static int CalculateDamage(ICombatCard attacker, ICombatCard defender, MoveType moveType) =>
        CalculateDamage(attacker.Statistics, defender.Statistics, moveType);

    public static int CalculateDamage(StatisticPointGroup attackerStats, StatisticPointGroup defenderStats, MoveType moveType)
    {
        var damage = moveType is MoveType.Attack ? attackerStats.Attack.CalculateValue() : attackerStats.Power.CalculateValue();
        var hpValue = defenderStats.HP.CalculateValue();

        return damage;
    }
}
