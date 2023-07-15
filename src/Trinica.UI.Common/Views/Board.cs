using Corelibs.Blazor.UIComponents.Common;
using Microsoft.AspNetCore.Components;
using Trinica.UseCases.Gameplay;

using GameEntity = Trinica.Entities.Gameplay.Game;

namespace Trinica.UI.Common.Views;

public partial class Board : BaseElement
{
    public delegate Task OnActionButtonClickDelegate(ActionType actionType);

    [Parameter] public GetGameStateQueryResponse? Game { get; set; }
    [Parameter] public OnActionButtonClickDelegate? OnActionButtonClick { get; set; }
    [Parameter] public Card.OnCardClickDelegate? OnCardClick { get; set; }

    public IEnumerable<Card> Cards => _cards.Values;
    private Dictionary<string, Card> _cards = new();

    private string _actionHint = "";
    private bool _wholeCardClickOnly;

    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            ;// return;
        
        await SetState(firstRender);
    }

    private async Task OnCardClickInternal(string cardId, Card.CardDeckType deckType)
    {
        if (OnCardClick is not null)
            await OnCardClick?.Invoke(cardId, deckType);
    }

    private async Task SetState(bool firstRender)
    {
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

            await InvokeAsync(StateHasChanged);
        }
    }


    public Task GreyOutNonDeckCards()
    {
        var cardsToGrayOut = Cards.Where(c =>
        {
            if (c.DeckType == Card.CardDeckType.CommonDeck)
                return false;

            if (c.DeckType == Card.CardDeckType.OwnDeck && c.PlayerId == Game.Player.PlayerId)
                return false;

            return true;
        })
        .ToArray();

        return Task.WhenAll(cardsToGrayOut.Select(c => c.SetGreyOut()).ToArray());
    }

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

    public record ActionType(string Value)
    {
        public static readonly ActionType TakeCardToHand = new("Take Card To Hand");
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
