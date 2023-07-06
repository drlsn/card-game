using System.Xml.Linq;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;
using Trinica.Entities.Users;

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
            hp:     new(25),
            speed:  new(11),
            power:  new(10));

        var hero1CardId = new HeroCardId("hero-1");
        var hero1Card = new HeroCard(hero1CardId, hero1Stats);
        var player1Deck = new FieldDeck();

        var deck1Id = new DeckId("deck-1");
        var player1Id = new UserId("player-1");
        var player1 = new Player(player1Id, deck1Id, hero1Card, player1Deck);

        // Player 2
        var hero2Stats = new StatisticPointGroup(
            attack: new(10),
            hp: new(20),
            speed: new(10),
            power: new(10));

        var hero2CardId = new HeroCardId("hero-2");
        var hero2Card = new HeroCard(hero2CardId, hero2Stats);
        var player2Deck = new FieldDeck();

        var deck2Id = new DeckId("deck-2");
        var player2Id = new UserId("player-2");
        var player2 = new Player(player2Id, deck2Id, hero2Card, player2Deck);

        // Game
        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

        // TakeCardsToCommonPool
        var random = new Random(1);
        Assert.IsTrue(game.TakeCardsToCommonPool(random));
        Assert.That(game.CommonPool.Count, Is.EqualTo(0));
        Assert.IsFalse(game.CanDo(game.TakeCardsToCommonPool));

        // TakeCardsToHand
        Assert.IsTrue(game.CanDo(game.TakeCardsToHand, player1Id));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand, player2Id));

        Assert.IsTrue(game.TakeCardsToHand(player1Id, Array.Empty<CardToTake>(), random));
        Assert.IsTrue(game.TakeCardsToHand(player2Id, Array.Empty<CardToTake>(), random));

        Assert.That(game.Players[0].HandDeck.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].HandDeck.Count, Is.EqualTo(0));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand));

        // CalculateLayDownOrderPerPlayer
        game.CalculateLayDownOrderPerPlayer();
        Assert.That(game.CardsLayOrderPerPlayer[0], Is.EqualTo(player1Id));
        Assert.That(game.CardsLayOrderPerPlayer[1], Is.EqualTo(player2Id));

        game.LayCardsToBattle(player1Id, Array.Empty<CardToLay>());
        game.LayCardsToBattle(player2Id, Array.Empty<CardToLay>());
        Assert.That(game.Players[0].BattlingDeck.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].BattlingDeck.Count, Is.EqualTo(0));

        game.PlayDices(player1Id, () => new Random(2));
        game.PlayDices(player2Id, () => new Random(2));
        Assert.That(game.Players[0].FreeDiceOutcomes.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].FreeDiceOutcomes.Count, Is.EqualTo(1));

        game.AssignDiceToCard(player1Id, diceIndex: 0, hero1CardId);
        game.AssignDiceToCard(player2Id, diceIndex: 0, hero2CardId);
        Assert.That(game.Players[0].FreeDiceOutcomes.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].FreeDiceOutcomes.Count, Is.EqualTo(0));
        Assert.That(game.Players[0].CardAssignments.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].CardAssignments.Count, Is.EqualTo(1));

        game.AssignCardTarget(player1Id, hero1CardId, hero2CardId);
        game.AssignCardTarget(player2Id, hero2CardId, hero1CardId);
        Assert.That(game.Players[0].CardAssignments[hero1CardId].TargetCardIds.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].CardAssignments[hero2CardId].TargetCardIds.Count, Is.EqualTo(1));

        game.StartRound(random);

        Assert.IsTrue(game.IsRoundOngoing());
        game.PerformMove(random);
        Assert.That(game.Players[1].HeroCard.Statistics.HP.CalculateValue(), Is.EqualTo(10));

        Assert.IsTrue(game.IsRoundOngoing());
        game.PerformMove(random);
        Assert.That(game.Players[0].HeroCard.Statistics.HP.CalculateValue(), Is.EqualTo(15));

        Assert.IsFalse(game.IsRoundOngoing());

        game.FinishRound(random);
    }
}
