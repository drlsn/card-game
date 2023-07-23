using Corelibs.Basic.Collections;
using Corelibs.Basic.Colors;
using Corelibs.Blazor.UIComponents.Common;
using Microsoft.AspNetCore.Components;
using System.Drawing;
using Trinica.Entities.Gameplay.Events;
using Trinica.UI.Common.State;
using Trinica.UseCases.Gameplay;

using GameEntity = Trinica.Entities.Gameplay.Game;

namespace Trinica.UI.Common.Views;

public partial class Board : BaseElement
{
    public delegate Task OnActionButtonClickDelegate(int index, ActionButtonDTO dto);
    public delegate Task OnLayCardDownDelegate(string cardId, string? targetCardId = "", bool? toCenter = false);
    public delegate Task OnAssignDiceDelegate(int diceIndex, string targetCardId);
    public delegate Task OnRemoveDiceDelegate(string cardId);
    public delegate Task OnAssignCardTargetDelegate(string cardId, string targetCardId);

    [Parameter] public GetGameStateQueryResponse? Game { get; set; }
    [Parameter] public OnActionButtonClickDelegate? OnActionButtonClick { get; set; }
    [Parameter] public Card.OnCardClickDelegate? OnCardClick { get; set; }
    [Parameter] public OnLayCardDownDelegate OnLayCardDown { get; set; }
    [Parameter] public OnAssignDiceDelegate OnAssignDice { get; set; }
    [Parameter] public OnRemoveDiceDelegate OnRemoveDice { get; set; }
    [Parameter] public Func<Task> OnConfirmDiceAssigns { get; set; }
    [Parameter] public OnAssignCardTargetDelegate OnAssignCardTarget { get; set; }
    [Parameter] public Func<Task> OnConfirmTargetAssigns { get; set; }

    [Inject] public GameState State { get; set; }

    private string[] _actions => Game.State.ExpectedActionTypes;

    public IEnumerable<Card> Cards => _cards.Values;
    private Dictionary<string, Card> _cards = new();

    private string _actionHint = "";
    private bool _wholeCardClickOnly;

    private readonly List<ActionButtonDTO> _actionButtons = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            ;// return;
        
        await SetState(firstRender);
    }

    private async Task OnActionButtonClickInternal(int index, ActionButtonDTO dto)
    {
        if (IsAssigningDice())
        {
            State.LastSelectedActionButton = new(index, dto);

            if (dto.Id == nameof(GameEntity.ConfirmAssignDicesToCards))
                await OnConfirmDiceAssigns.Invoke();
                
            return;
        }
        if (IsAssigningTarget())
        {
            State.LastSelectedActionButton = new(index, dto);

            if (dto.Id == nameof(GameEntity.ConfirmCardTargets))
                await OnConfirmTargetAssigns.Invoke();

            return;
        }

        await OnActionButtonClick?.Invoke(index, dto);
    }

    private async Task OnCardClickInternal(CardDTO cardDTO, string playerId, Card.CardDeckType deckType)
    {
        if (OnCardClick is null)
            return;

        var lastSelectedCard = State.LastSelectedCard;
        State.LastSelectedCard = cardDTO;

        if (IsLayDown())
        {
            if (playerId != Game.Player.PlayerId)
                return;

            if (deckType == Card.CardDeckType.BattlingDeck &&
                (cardDTO.Type == "unit" || cardDTO.Type == "hero") &&
                (cardDTO.Type == "skill" || cardDTO.Type == "item"))
            {
                await OnLayCardDown?.Invoke(State.LastSelectedCard.Id, cardDTO.Id);
                return;
            }

            State.IsLayingCardToTarget = false;

            if (deckType == Card.CardDeckType.HandDeck &&
                (cardDTO.Type == "skill" || cardDTO.Type == "item"))
            {
                State.IsLayingCardToTarget = true;
                await InvokeAsync(StateHasChanged);
                return;
            }
        }
        else
        if (IsAssigningDice())
        {
            var cardAssignment = Game.Player.CardAssignments.FirstOrDefault(c => c.SourceCardId == cardDTO.Id);
            if (cardAssignment is not null && cardAssignment.DiceOutcome is not null)
            {
                if (State.LastSelectedActionButton is not null)
                    return;

                await OnRemoveDice?.Invoke(cardDTO.Id);
            }

            if (State.LastSelectedActionButton is null)
                return;

            var btn = State.LastSelectedActionButton;
            State.LastSelectedActionButton = null;
            State.LastSelectedCard = null;

            await OnAssignDice?.Invoke(btn.index, cardDTO.Id);
            return;
        }
        if (IsAssigningTarget())
        {
            if (lastSelectedCard is not null)
            {
                State.LastSelectedCard = null;
                await OnAssignCardTarget?.Invoke(lastSelectedCard.Id, cardDTO.Id);
                return;
            }
        }

        await OnCardClick?.Invoke(cardDTO, playerId, deckType);
    }

    private async Task SetState(bool firstRender)
    {
        _actionButtons.Clear();

        var haveToWait = DoesHaveToWaitForAnotherPlayer();
        if (haveToWait)
            _actionHint = "Wait for another player action";

        if (IsTakingCards())
        {
            if (!haveToWait && Game.Player.HandDeck.Cards.Length < 6)
                _actionHint = "Select Deck to take a card from!";

            if (!haveToWait && Game.Enemies.First().HandDeck.Cards.Length < 6 &&
                Game.Player.HandDeck.Cards.Length == 6)
                _actionHint = "Wait for another player action";

            if (firstRender)
                await GreyOutNonDeckCards();
        }
        else
        if (IsCalcLayDown())
        {
            _actionHint = "Calculating which player should start laying cards first...";
        }
        else
        if (IsLayDown())
        {
            var i = Array.FindIndex(Game.State.ExpectedPlayers, id => id == Game.Player.PlayerId);
            if (Game.State.AlreadyMadeActionsPlayers.Length == i)
            {
                _actionButtons.Add(new(nameof(GameEntity.PassLayCardToBattle), "Pass"));
            }

            await ClearOutAllCards();
            if (!haveToWait)
            {
                if (State.IsLayingCardToTarget)
                {
                    _actionHint = $"Select Target Card for the {State.LastSelectedCard.Type} or continue";
                }
                else
                if (Game.Player.BattlingDeck.Cards.Length < 6)
                {
                    _actionHint = "Lay The Cards Down or Skip";
                }
            }
        }
        else
        if (IsRolling() || IsRerolling())
        {
            if (IsRolling() && !IsPlayerDone())
            {
                _actionButtons.Add(new(nameof(GameEntity.PlayDices), "Roll Dices"));
            }

            if (IsRerolling() || IsPlayerDone())
            {
                Game.Player.DiceOutcomes.ForEach((a, i) =>
                    _actionButtons.Add(new($"dice-{i}", a.Value)
                    {
                        Interactable = false
                    }));
            }

            if (IsRerolling() && !IsPlayerDone())
            {
                _actionButtons.Add(new(nameof(GameEntity.PassReplayDices), "Move On"));
            }

            if (!IsPlayerDone())
                _actionHint = "Reroll or pass";
        }
        else
        if (IsAssigningDice())
        {
            Game.Player.DiceOutcomes.ForEach((a, i) =>
                _actionButtons.Add(new($"dice-{i}", a.Value)));

            if (State.LastSelectedActionButton is not null &&
                State.LastSelectedActionButton.index >= 0 &&
                State.LastSelectedActionButton.index < _actionButtons.Count - 1)
                _actionButtons[State.LastSelectedActionButton.index].Color = Color.LightSeaGreen.ToHexString();

            Game.Player.CardAssignments.ForEach(a =>
            {
                if (a.DiceOutcome is null ||
                    a.DiceOutcomeIndex == -1)
                    return;

                var allBattlingCards = Game.Player.BattlingDeck.Cards.Prepend(Game.Player.Hero).ToList();
                int cardIndex = allBattlingCards.FindIndex(c => c.Id == a.SourceCardId);
                _actionButtons[a.DiceOutcomeIndex].Color = Color.LightGreen.ToHexString();
                _actionButtons[a.DiceOutcomeIndex].Name += $"-{cardIndex + 1}";
            });

            _actionButtons.Add(new(nameof(GameEntity.ConfirmAssignDicesToCards), "Confirm"));

            if (!IsPlayerDone())
                _actionHint = "Assign dices to cards";
        }
        else
        if (IsAssigningTarget())
        {
            if (!IsPlayerDone())
                _actionHint = "Assign card targets";

            Game.Player.CardAssignments.ForEach(a =>
            {
                if (a.TargetCardIds.IsNullOrEmpty())
                    return;

                if (!_cards.TryGetValue(a.SourceCardId, out Card card))
                    return;
            });

            _actionButtons.Add(new(nameof(GameEntity.ConfirmCardTargets), "Confirm"));
        }

        await InvokeAsync(StateHasChanged);
    }

    public Task GreyOutNonDeckCards()
    {
        var cardsToGrayOut = Cards
            .Where(c => c.DeckType != Card.CardDeckType.CommonDeck && c.DeckType != Card.CardDeckType.OwnDeck)
            .ToArray();

        return Task.WhenAll(cardsToGrayOut.Select(c => c.SetGreyOut()).ToArray());
    }

    public Task ClearOutAllCards() =>Task.WhenAll(Cards.Select(c => c.SetGreyOut(false)).ToArray());

    private bool DoesHaveToWaitForAnotherPlayer()
    {
        var state = Game.State;
        if (state.MustObeyOrder)
        {
            var i = state.AlreadyMadeActionsPlayers.Length;
            var nextPlayerToMakeAction = state.ExpectedPlayers[i];
            if (nextPlayerToMakeAction == Game.Player.PlayerId)
                return false;

            return true;
        }

        return state.AlreadyMadeActionsPlayers.Contains(Game.Player.PlayerId);
    }

    private bool IsPlayerDone() => Game.State.AlreadyMadeActionsPlayers.Contains(Game.Player.PlayerId);
    private bool IsEnemyDone() => Game.State.AlreadyMadeActionsPlayers.Contains(Game.Enemies[0].PlayerId);

    private bool IsTakingCards() => _actions.Contains(nameof(GameEntity.TakeCardToHand));
    private bool IsCalcLayDown() => _actions.Contains(nameof(GameEntity.CalculateLayDownOrderPerPlayer));
    private bool IsLayDown() => _actions.Contains(nameof(GameEntity.LayCardToBattle));
    private bool IsRolling() => _actions.Contains(nameof(GameEntity.PlayDices));
    private bool IsRerolling() => _actions.Contains(nameof(GameEntity.PassReplayDices));
    private bool IsAssigningDice() => _actions.Contains(nameof(GameEntity.AssignDiceToCard));
    private bool IsAssigningTarget() => _actions.Contains(nameof(GameEntity.AssignCardTarget));

    private string[] GetCardTargets(CardDTO cardDto, string playerId)
    {
        if (playerId.IsNullOrEmpty() || playerId != Game.Player.PlayerId)
            return Array.Empty<string>();

        var cardTargetName = "";
        var cardAssign = Game.Player.CardAssignments.FirstOrDefault(c => c.SourceCardId == cardDto.Id);
        if (cardAssign is not null && !cardAssign.TargetCardIds.IsNullOrEmpty())
        {
            var targetCard = Game.Enemies[0]
                .BattlingDeck.Cards.Prepend(Game.Enemies[0].Hero)
                .Select((card, i) => new { card, i })
                .FirstOrDefault(c => c.card.Id == cardAssign.TargetCardIds[0]);

            cardTargetName = (targetCard.i + 1).ToString();
        }

        return new string[] { cardTargetName };
    }

    public class ActionButtonDTO
    {
        public ActionButtonDTO(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public bool Interactable { get; set; } = true;
    }
}

public static class DeckType_To_CardSource_Converter
{
    public static string ToCardSourceStr(this Card.CardDeckType deckType)
    {
        if (deckType == Card.CardDeckType.CommonDeck)
            return "common-pool";

        if (deckType == Card.CardDeckType.OwnDeck)
            return "own";

        return "";
    }
}
