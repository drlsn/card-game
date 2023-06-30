using CardGame.Entities.Decks;
using CardGame.Entities.Gameplay.Cards;
using System.Text.Json.Serialization;

namespace CardGame.Entities.Gameplay;

public class EntireDeck
{
    public DeckId Id { get; private set; }
    public HeroCard HeroCard { get; private set; }
    public FieldDeck FieldDeck { get; private set; }

    [JsonConstructor]
    public EntireDeck(
        DeckId id,
        HeroCard heroCard,
        FieldDeck fieldCards)
    {
        Id = id;
        HeroCard = heroCard;
        FieldDeck = fieldCards;
    }

    public void ShuffleAll(Random random) =>
        FieldDeck.ShuffleAll(random);

    public FieldDeck TakeNCards(Random random, int n) =>
        FieldDeck.TakeNCards(random, n);
}
