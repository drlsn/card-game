using Corelibs.Basic.UseCases.DTOs;
using Corelibs.Blazor.UIComponents.Common;
using Microsoft.AspNetCore.Components;
using Trinica.Entities.Gameplay.Events;
using Trinica.UI.Common.State;
using Trinica.UseCases.Gameplay;

using GameEntity = Trinica.Entities.Gameplay.Game;

namespace Trinica.UI.Common.Views;

public partial class Board : BaseElement
{
    public delegate Task OnActionButtonClickDelegate(string actionName);
    public delegate Task OnLayCardDownDelegate(string cardId, string? targetCardId = "", bool? toCenter = false);

    [Parameter] public GetGameStateQueryResponse? Game { get; set; }
    [Parameter] public OnActionButtonClickDelegate? OnActionButtonClick { get; set; }
    [Parameter] public Card.OnCardClickDelegate? OnCardClick { get; set; }
    [Parameter] public OnLayCardDownDelegate OnLayCardDown { get; set; }

    [Inject] public GameState State { get; set; }

    public IEnumerable<Card> Cards => _cards.Values;
    private Dictionary<string, Card> _cards = new();

    private string _actionHint = "";
    private bool _wholeCardClickOnly;

    private readonly List<IdentityDTO> _actionButtons = new();

    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            ;// return;
        
        await SetState(firstRender);
    }

    private async Task OnCardClickInternal(CardDTO cardDTO, string playerId, Card.CardDeckType deckType)
    {
        if (OnCardClick is null)
            return;

        State.LastSelectedCard = cardDTO;

        var actions = Game.State.ExpectedActionTypes;
        if (actions.Contains(nameof(GameEntity.LayCardToBattle)))
        {
            if (playerId != Game.Player.PlayerId)
                return;

            if (deckType == Card.CardDeckType.BattlingDeck &&
                (cardDTO.Type == "unit" || cardDTO.Type == "hero") &&
                (State.LastSelectedCard.Type == "skill" || State.LastSelectedCard.Type == "item"))
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

        await OnCardClick?.Invoke(cardDTO, playerId, deckType);
    }

    private async Task SetState(bool firstRender)
    {
        _actionButtons.Clear();

        var haveToWait = DoesHaveToWaitForAnotherPlayer();
        if (haveToWait)
            _actionHint = "Wait for another player action";

        var actions = Game.State.ExpectedActionTypes;
        if (actions.Contains(nameof(GameEntity.TakeCardToHand)))
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
        if (actions.Contains(nameof(GameEntity.CalculateLayDownOrderPerPlayer)))
        {
            _actionHint = "Calculating which player should start laying cards first...";
        }
        else
        if (actions.Contains(nameof(GameEntity.LayCardsToBattle)))
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
        if (actions.Contains(nameof(GameEntity.PlayDices)))
        {
            if (!Game.State.AlreadyMadeActionsPlayers.Contains(Game.Player.PlayerId))
                _actionHint = "Play Dices";
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
