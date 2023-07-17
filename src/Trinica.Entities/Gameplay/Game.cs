using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using System.Linq;
using System.Numerics;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>, IAggregateRoot<GameId>
{
    public const string DefaultCollectionName = "games";

    public Player[] Players { get; private set; }
    public FieldDeck CommonPool { get; private set; }
    public CardAndOwner CenterCard { get; private set; }
    public int CenterCardRoundsAlive { get; private set; }
    public UserId[] CardsLayOrderPerPlayer { get; private set; }

    [Ignore]
    public RoundSettings RoundSettings { get; private set; } = new();

    public IActionController ActionController { get; private set; } = new GameActionController();

    public Game(
        GameId id,
        Player[] players) : base(id)
    {
        Players = players;
        ActionController.SetActionExpectedNext(StartGame).By(Players.ToIds());
    }

    public bool StartGame(UserId playerId, Random random = null)
    {
        if (!ActionController.CanMakeAction(StartGame, playerId))
            return false;

        return ActionController
            .SetActionDone(StartGame, playerId)
            .SetActionExpectedNext(TakeCardsToCommonPool)
            .IsSuccess;
    }

    public bool TakeCardsToCommonPool(Random random = null)
    {
        if (!ActionController.CanMakeAction(TakeCardsToCommonPool))
            return false;

        CommonPool = Players.ShuffleAllAndTakeHalfCards(random ?? new());

        return ActionController
            .SetActionDone(TakeCardsToCommonPool)
            .SetActionExpectedNext(TakeCardsToHand)
            .Or(TakeCardToHand, ActionRepeat.Multiple)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool TakeCardToHand(UserId playerId, CardToTake card, Random random = null) 
    {
        if (!ActionController.CanMakeAction(TakeCardToHand, playerId))
            return false;

        var player = Players.OfId(playerId);
        if (!player.CanTakeCardToHand(out int max, CommonPool.Count))
            return false;

        if (card.Source == CardSource.CommonPool)
            player.AddCardToHand(CommonPool.TakeCard(random ?? new()));
        else
            if (card.Source == CardSource.Own)
            player.TakeCardToHand(random ?? new());

        if (!player.CanTakeCardToHand(out max, CommonPool.Count))
            return ActionController
                .SetActionDone(TakeCardsToHand, playerId)
                .SetActionExpectedNext(CalculateLayDownOrderPerPlayer)
                .IsSuccess;
        
        return ActionController.SetActionDone(TakeCardToHand, playerId).IsSuccess;
    }

    public bool TakeCardsToHand(UserId playerId, CardToTake[] cards, Random random = null)
    {
        if (!ActionController.CanMakeAction(TakeCardsToHand, playerId))
            return false;

        if (cards.IsNullOrEmpty())
            return SetNextAction();

        var player = Players.OfId(playerId);

        if (!player.CanTakeCardToHand(out int max, CommonPool.Count) || max < cards.Length)
            return false;

        cards.ForEach(card =>
        {
            if (!player.CanTakeCardToHand(out int max, CommonPool.Count))
                return;

            if (card.Source == CardSource.CommonPool)
                player.AddCardToHand(CommonPool.TakeCard(random ?? new()));
            else
            if (card.Source == CardSource.Own)
                player.TakeCardToHand(random ?? new());
        });

        return SetNextAction();

        bool SetNextAction() => 
            ActionController
                .SetActionDone(TakeCardsToHand, playerId)
                .SetActionExpectedNext(CalculateLayDownOrderPerPlayer)
                .IsSuccess;
    }
     
    public bool CalculateLayDownOrderPerPlayer()
    {
        if (!ActionController.CanMakeAction(CalculateLayDownOrderPerPlayer))
            return false;

        CardsLayOrderPerPlayer = Players
            .GetPlayersOrderedByHeroSpeed()
            .ToIds();

        return ActionController
            .SetActionDone(CalculateLayDownOrderPerPlayer)
            .SetActionExpectedNext(LayCardToBattle, ActionRepeat.Multiple)
            .Or(LayCardsToBattle)
            .Or(PassLayCardToBattle)
            .By(CardsLayOrderPerPlayer, mustObeyOrder: true)
            .IsSuccess;
    }

    public bool CanLayCardDown(UserId playerId)
    {
        var player = Players.OfId(playerId);
        if (player.CanLayCardDownToTarget(CenterCard))
            return true;

        return player.CanLayCardDown();
    }

    public bool LayCardToBattle(UserId playerId, CardToLay card)
    {
        if (!ActionController.CanMakeAction(LayCardToBattle, playerId))
            return false;

        var player = Players.OfId(playerId);
        var cards = TryLayCardToCenter(player, new[] { card });
        if (!cards.IsNullOrEmpty())
            if (!player.LayCardsToBattle(cards))
                return false;

        if (!player.CanLayCardDown())
            return ActionController
                .SetActionDone(LayCardsToBattle, playerId)
                .SetActionExpectedNext(nameof(PlayDices))
                .By(Players.ToIds())
                .IsSuccess;

        return ActionController.SetActionDone(LayCardToBattle, playerId).IsSuccess;
    }

    public bool LayCardsToBattle(UserId playerId, CardToLay[] cards)
    {
        if (!ActionController.CanMakeAction(LayCardsToBattle, playerId))
            return false;

        var player = Players.OfId(playerId);
        cards = TryLayCardToCenter(player, cards);

        if (!player.LayCardsToBattle(cards))
            return false;

        return ActionController
            .SetActionDone(LayCardsToBattle, playerId)
            .SetActionExpectedNext(nameof(PlayDices))
            .By(Players.ToIds())
            .IsSuccess;
    }

    private CardToLay[] TryLayCardToCenter(Player player, CardToLay[] cards)
    {
        var cardToCenterToLay = cards.FirstOrDefault(c => c.ToCenter);
        if (cardToCenterToLay is null)
            return cards;

        var handCards = player.HandDeck.GetAllCards();
        var cardToCenter = handCards.FirstOrDefault(c => c.Id == cardToCenterToLay.SourceCardId);
        if (cardToCenter is null)
            return cards;

        if (CenterCard is not null &&
            CenterCard.PlayerId == player.Id &&
            CenterCard.Card is ICardWithSlots centerCardWithSlots)
        {
            if (cardToCenter is SkillCard || cardToCenter is ItemCard)
            {
                var slots = centerCardWithSlots.Slots;
                if (slots.AddCard(cardToCenter))
                {
                    player.TakeCardFromHand(cardToCenter.Id);
                    return cards;
                }
            }
        }

        if (cardToCenter is not UnitCard)
            return cards;

        CenterCard = new(player.TakeCardFromHand(cardToCenter.Id), player.Id);
        cards = cards.Except(cardToCenterToLay).ToArray();
        CenterCardRoundsAlive = 0;

        return cards;
    }

    public bool PassLayCardToBattle(UserId playerId)
    {
        if (!ActionController.CanMakeAction(PassLayCardToBattle))
            return false;

        return ActionController
            .SetActionDone(PassLayCardToBattle, playerId)
            .SetActionExpectedNext(nameof(PlayDices))
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool PlayDices(UserId playerId, Random random) =>
        PlayDices(playerId, () => random);

    public bool PlayDices(UserId playerId, Func<Random> getRandom)
    {
        if (!ActionController.CanMakeAction(nameof(PlayDices), playerId))
            return false;

        var player = Players.OfId(playerId);
        player.PlayDices(getRandom);

        return ActionController
            .SetActionDone(nameof(PlayDices), playerId)
            .SetActionExpectedNext(ReplayDices)
            .Or(PassReplayDices)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool PassReplayDices(UserId playerId)
    {
        if (!ActionController.CanMakeAction(PassReplayDices, playerId))
            return false;

        return ActionController
            .SetActionDone(PassReplayDices, playerId)
            .SetActionExpectedNext(AssignDiceToCard, ActionRepeat.Multiple)
            .Or(RemoveDiceFromCard, ActionRepeat.Multiple)
            .Or(ConfirmAssignDicesToCards)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool ReplayDices(UserId playerId, int n, Func<Random> getRandom)
    {
        if (!ActionController.CanMakeAction(ReplayDices, playerId))
            return false;

        var player = Players.OfId(playerId);
        player.PlayDices(n, getRandom);

        return ActionController
            .SetActionDone(ReplayDices, playerId)
            .SetActionExpectedNext(AssignDiceToCard, ActionRepeat.Multiple)
            .Or(RemoveDiceFromCard, ActionRepeat.Multiple)
            .Or(ConfirmAssignDicesToCards)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool AssignDiceToCard(UserId playerId, int diceIndex, CardId cardId)
    {
        if (!ActionController.CanMakeAction(AssignDiceToCard, playerId))
            return false;

        var player = Players.OfId(playerId);
        if (!player.AssignDiceToCard(diceIndex, cardId))
            return false;

        return ActionController
            .SetActionDone(AssignDiceToCard, playerId)
            .IsSuccess;
    }

    public bool RemoveDiceFromCard(UserId playerId, CardId cardId)
    {
        if (!ActionController.CanMakeAction(RemoveDiceFromCard, playerId))
            return false;

        var player = Players.OfId(playerId);
        player.RemoveDiceFromCard(cardId);

        return ActionController
           .SetActionDone(RemoveDiceFromCard, playerId)
           .IsSuccess;
    }

    public bool ConfirmAssignDicesToCards(UserId playerId)
    {
        return ActionController
            .SetActionDone(ConfirmAssignDicesToCards, playerId)
            .SetActionExpectedNext(ChooseCardSkill, ActionRepeat.Multiple)
            .Or(AssignCardTarget, ActionRepeat.Multiple)
            .Or(RemoveCardTarget, ActionRepeat.Multiple)
            .Or(ConfirmCardTargets)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool ChooseCardSkill(UserId playerId, CardId cardId, int skillIndex)
    {
        if (!ActionController.CanMakeAction(ChooseCardSkill, playerId))
            return false;

        var player = Players.OfId(playerId);
        player.ChooseCardSkill(cardId, skillIndex);

        return ActionController
            .SetActionDone(ChooseCardSkill, playerId)
            .IsSuccess;
    }

    public bool AssignCardTarget(UserId playerId, CardId cardId, CardId targetCardId)
    {
        if (!ActionController.CanMakeAction(AssignCardTarget, playerId))
            return false;

        var player = Players.OfId(playerId);
        player.AssignCardTarget(cardId, targetCardId);

        return ActionController
            .SetActionDone(AssignCardTarget, playerId)
            .IsSuccess;
    }

    public bool RemoveCardTarget(UserId playerId, CardId cardId, CardId targetCardId)
    {
        if (!ActionController.CanMakeAction(RemoveCardTarget, playerId))
            return false;

        var player = Players.OfId(playerId);
        player.RemoveCardTarget(cardId, targetCardId);

        return ActionController
            .SetActionDone(RemoveCardTarget, playerId)
            .IsSuccess;
    }

    public bool ConfirmCardTargets(UserId playerId)
    {
        if (!ActionController.CanMakeAction(ConfirmCardTargets, playerId))
            return false;

        return ActionController
            .SetActionDone(ConfirmCardTargets, playerId)
            .SetActionExpectedNext(StartRound)
            .IsSuccess;
    }

    public bool StartRound(Random random)
    {
        if (!ActionController.CanMakeAction(StartRound))
            return false;

        if (CenterCard is not null)
        {
            CenterCardRoundsAlive++;
            if (CenterCardRoundsAlive >= 6)
                return ActionController
                    .SetActionDone(StartRound)
                    .SetActionExpectedNext(FinishGame)
                    .IsSuccess;
        }

        _cardIndex = 0;
        _cards = Players.GetBattlingCardsBySpeed(random);
        _cards.ForEach(card =>
        {
            if (card is not ICombatCard combatCard)
                return;

            combatCard.Effects.ForEach(effect =>
                effect.OnRoundStart(combatCard, null, null, RoundSettings));
        });

        return ActionController
            .SetActionDone(StartRound)
            .SetActionExpectedNext(PerformMove, ActionRepeat.Multiple)
            .Or(PerformRound)
            .IsSuccess;
    }

    private ICard[] _cards;
    private int _cardIndex;
    public bool PerformMove(Random random)
    {
        if (!ActionController.CanMakeAction(PerformMove))
            return false;

        var card = _cards[_cardIndex];
        _cardIndex++;

        var player = Players.GetPlayerWithCard(card.Id);
        var cardAssignment = player.CardAssignments[card.Id];
        var targetCards = _cards.Where(c => cardAssignment.TargetCardIds.Contains(c.Id)).Cast<ICombatCard>().ToArray();
        targetCards = targetCards.Where(c => !RoundSettings.NotAllowedAsTargetCards.ContainsKey(c.Id.Value)).ToArray();
        if (targetCards.IsEmpty())
            return true;

        var otherPlayers = Players.NotOfId(player.Id);
        var enemiesBattlingCards = otherPlayers.GetBattlingCards().OfType<ICombatCard>().ToArray();

        if (!RoundSettings.PrioritizedToAttackCards.IsNullOrEmpty())
        {
            var cardId = RoundSettings.PrioritizedToAttackCards.Values.Shuffle(random).First();
            targetCards = enemiesBattlingCards.Where(c => c.Id == cardId).ToArray();
        }

        if (card is not ICombatCard combatCard)
            return true;

        if (cardAssignment.DiceOutcome is null)
            return false;

        var moveType = cardAssignment.DiceOutcome.IsElement() ? MoveType.Skill : MoveType.Attack;

        // ----- use items! -----
        var cardWithItems = combatCard as ICardWithItems;
        if (cardWithItems is not null)
            cardWithItems.ItemCards.ForEach(itemCard =>
                combatCard.Statistics.Modify(itemCard.Statistics, itemCard.Id.Value));

        targetCards.ForEach(targetCard =>
        {
            var cardWithItems = targetCard as ICardWithItems;
            if (cardWithItems is not null)
                cardWithItems.ItemCards.ForEach(itemCard =>
                    targetCard.Statistics.Modify(itemCard.Statistics, itemCard.Id.Value));
        });

        // Attacker - BeforeMoveAtAll
        var moveAtAll = new Move()
        {
            Damage = CalculateDamage(combatCard, null, moveType, cardAssignment.SkillIndex),
            Type = moveType
        };
        combatCard.Effects.ForEach(effect =>
            effect.BeforeMoveAtAll(combatCard, new(targetCards, enemiesBattlingCards, null), moveAtAll));

        var movesAtSingle = new Dictionary<CardId, Move>();
        targetCards.ForEach(targetCard =>
        {
            var damage = CalculateDamage(combatCard, targetCard, moveType, cardAssignment.SkillIndex);
            movesAtSingle.Add(targetCard.Id, new Move()
            {
                Damage = moveAtAll.Damage,
                Type = moveType
            });
        });

        // Attacker - BeforeMoveAtSingleTarget
        combatCard.Effects.ForEach(effect =>
           targetCards.ForEach(targetCard =>
           {
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
                combatCard.Statistics.RemoveAll(itemCard.Id.Value));

        // ------------------------
        // PERFORM ACTION!!!
        // ------------------------
        if (moveAtAll.MoveEnabled)
        {
            if (moveType is MoveType.Attack && 
                moveAtAll.AttackEnabled &&
                (card is HeroCard || card is UnitCard))
            {
                foreach (var targetCard in targetCards)
                {
                    var move = movesAtSingle[targetCard.Id];

                    var targetPlayer = Players.GetPlayerWithCard(targetCard.Id);
                    targetPlayer.InflictDamage(move.Damage, targetCard.Id);
                    if (targetPlayer.IsCardDead(targetCard))
                    {
                        if (targetCard is HeroCard)
                            return ActionController
                                .SetActionDone(PerformRound)
                                .SetActionExpectedNext(FinishGame)
                                .IsSuccess;

                        if (CenterCard == targetCard)
                            CenterCardRoundsAlive = 0;
                    }
                }
            }
            else
            if (moveType is MoveType.Skill && moveAtAll.SkillsEnabled)
            {
                foreach (var targetCard in targetCards)
                {
                    var move = movesAtSingle[targetCard.Id];
                    if (!move.SkillsEnabled)
                        continue;

                    var targetPlayer = Players.GetPlayerWithCard(targetCard.Id);
                    if (combatCard.DoesPowerDamage(cardAssignment.SkillIndex))
                        targetPlayer.InflictDamage(move.Damage, targetCard.Id);

                    if (combatCard is SpellCard)
                        player.KillCard(combatCard.Id);

                    if (targetPlayer.IsCardDead(targetCard))
                    {
                        if (targetCard is HeroCard)
                            return ActionController
                                .SetActionDone(PerformRound)
                                .SetActionExpectedNext(FinishGame)
                                .IsSuccess;

                        if (CenterCard == targetCard)
                            CenterCardRoundsAlive = 0;
                    }

                    if (move.EffectsEnabled)
                    {
                        var effects = combatCard.GetEffects(cardAssignment.SkillIndex);
                        targetCard.Effects.AddRange(effects);
                    }

                };
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
               var damage = CalculateDamage(combatCard, targetCard, moveType, cardAssignment.SkillIndex);
               effect.AfterMoveAtSingleTarget(combatCard, targetCard, new()
               {
                   Damage = damage,
                   Type = moveType
               });
           }));

        // Defender - AfterReceive
        targetCards.ForEach(targetCard =>
        {
            var damage = CalculateDamage(combatCard, targetCard, moveType, cardAssignment.SkillIndex);
            targetCard.Effects.ForEach(effect =>
                effect.AfterReceive(targetCard, new(combatCard, enemiesBattlingCards, null), new()
                {
                    Damage = 23,
                    Type = moveType
                }));
        });

        ActionController.SetActionDone(PerformMove);

        if (IsGameOver())
            return ActionController
                .SetActionDone(PerformRound)
                .SetActionExpectedNext(FinishGame)
                .IsSuccess;

        // ----- unuse items! -----
        if (cardWithItems is not null)
            cardWithItems.ItemCards.ForEach(itemCard =>
                combatCard.Statistics.RemoveAll(itemCard.Id.Value));

        if (!IsRoundOngoing())
            return ActionController
                .SetActionDone(PerformRound)
                .SetActionExpectedNext(FinishRound)
                .IsSuccess;

        return ActionController.SetActionExpectedNext(PerformMove).IsSuccess;
    }

    public bool PerformRound(Random random)
    {
        if (!ActionController.CanMakeAction(PerformRound))
            return false;

        _cards ??= Players.GetBattlingCardsBySpeed(random);

        while (IsRoundOngoing())
            if (!PerformMove(random))
                return false;

        return true;
    }

    public bool FinishRound(Random random)
    {
        if (!ActionController.CanMakeAction(FinishRound))
            return false;

        _cards.ForEach(card =>
        {
            if (card is not ICombatCard combatCard)
                return;

            combatCard.Effects.ForEach(effect =>
                effect.OnRoundFinish(combatCard));
        });

        return ActionController
            .SetActionDone(FinishRound)
            .SetActionExpectedNext(TakeCardsToHand)
            .Or(TakeCardToHand, ActionRepeat.Multiple)
            .By(Players.ToIds())
            .IsSuccess;
    }

    public bool IsGameOver() =>
        IsGameOverByHeroElimination() ||
        IsGameOverByCenterOccupied();

    public bool FinishGame(Random random)
    {
        if (!ActionController.CanMakeAction(FinishGame))
            return false;

        return ActionController
            .SetActionDone(FinishGame)
            .SetActionExpectedNext("None")
            .IsSuccess;
    }

    public bool IsGameOverByHeroElimination() => 
        Players.Any(p => p.HeroCard is null) ||
        Players.Any(p => p.HeroCard.Statistics.HP.CalculatedValue <= 0);

    public bool IsGameOverByCenterOccupied() => CenterCardRoundsAlive >= 6;

    public bool IsDead(ICard card) => Players.FirstOrDefault(p => p.DeadDeck.Contains(card)) is not null;

    public bool CanDo(Delegate @delegate, UserId userId = null) => ActionController.CanMakeAction(@delegate, userId);
    public bool CanDo(string type, UserId userId = null) => ActionController.CanMakeAction(type, userId);

    public bool IsRoundOngoing() => 
        _cards is not null &&
        _cardIndex < _cards.Length &&
        !IsGameOver();

    public static int CalculateDamage(ICombatCard attacker, ICombatCard defender, MoveType moveType, int skillIndex)
    {
        if (attacker is SpellCard spellCard && moveType == MoveType.Skill)
            return spellCard.Damage;

        return moveType switch
        {
            MoveType.Skill => attacker.Statistics.Power.CalculatedValue /* + skill.Damage */,
            MoveType.Attack => attacker.Statistics.Attack.CalculatedValue,
            _ => 0
        };
    }
}
