using Corelibs.Blazor.UIComponents.Common;
using Microsoft.AspNetCore.Components;
using Trinica.UseCases.Gameplay;

namespace Trinica.UI.Common.Components;

public partial class Board : BaseElement
{
    public delegate Task OnActionButtonClickDelegate(ActionType actionType);

    [Parameter] public GetGameStateQueryResponse? Game { get; set; }
    [Parameter] public OnActionButtonClickDelegate? OnActionButtonClick { get; set; }

    public IEnumerable<Card> Cards => _cards.Values;
    private Dictionary<string, Card> _cards = new();

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

    public record ActionType(string Value)
    {
        public static readonly ActionType TakeCardToHand = new("Take Card To Hand");
    }
}
