using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;

namespace Trinica.Entities.Tests;

public class GameplayTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Test()
    {
        // Player 1
        var hero1Stats = new StatisticPointGroup(
            attack: new(10),
            hp:     new(10),
            speed:  new(10),
            power:  new(10));

        var hero1CardId = new HeroCardId("hero-1");
        var hero1Card = new HeroCard(hero1CardId, hero1Stats);
        var player1Deck = new FieldDeck();

        var deck1Id = new DeckId("deck-1");
        var player1 = new Player(deck1Id, hero1Card, player1Deck);

        // Player 2
        var hero2Stats = new StatisticPointGroup(
            attack: new(10),
            hp: new(10),
            speed: new(10),
            power: new(10));

        var hero2CardId = new HeroCardId("hero-2");
        var hero2Card = new HeroCard(hero2CardId, hero2Stats);
        var player2Deck = new FieldDeck();

        var deck2Id = new DeckId("deck-2");
        var player2 = new Player(deck2Id, hero2Card, player2Deck);

        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

    }
}
