using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;
using Trinica.Entities.Users;

namespace Trinica.Entities.Tests;

public class GameplayTests
{
    [Test]
    public void PlayOnlyHeroesAndCheckControl()
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

        var random = new Random(1);

        // Game
        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

        // StartGame
        Assert.IsFalse(game.CanDo(game.StartGame));
        Assert.IsTrue(game.CanDo(game.StartGame, player1Id));
        Assert.IsTrue(game.CanDo(game.StartGame, player2Id));
        {
            Assert.IsTrue(game.StartGame(player1Id, random));
            Assert.IsTrue(game.StartGame(player2Id, random));
        }
        Assert.IsFalse(game.CanDo(game.StartGame));
        Assert.IsFalse(game.CanDo(game.StartGame, player1Id));
        Assert.IsFalse(game.CanDo(game.StartGame, player2Id));

        // TakeCardsToCommonPool
        Assert.IsTrue(game.TakeCardsToCommonPool(random));
        {
            Assert.That(game.CommonPool.Count, Is.EqualTo(0));
        }
        Assert.IsFalse(game.CanDo(game.TakeCardsToCommonPool));

        // TakeCardsToHand
        Assert.IsTrue(game.CanDo(game.TakeCardsToHand, player1Id));
        Assert.IsTrue(game.CanDo(game.TakeCardsToHand, player2Id));
        {
            Assert.IsTrue(game.TakeCardsToHand(player1Id, Array.Empty<CardToTake>(), random));
            Assert.IsTrue(game.TakeCardsToHand(player2Id, Array.Empty<CardToTake>(), random));
        }
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand, player1Id));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand, player2Id));
        Assert.That(game.Players[0].HandDeck.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].HandDeck.Count, Is.EqualTo(0));

        // CalculateLayDownOrderPerPlayer
        Assert.IsTrue(game.CanDo(game.CalculateLayDownOrderPerPlayer));
        {
            Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        }
        Assert.IsFalse(game.CanDo(game.CalculateLayDownOrderPerPlayer));
        Assert.That(game.CardsLayOrderPerPlayer[0], Is.EqualTo(player1Id));
        Assert.That(game.CardsLayOrderPerPlayer[1], Is.EqualTo(player2Id));

        // LayCardsToBattle
        Assert.IsFalse(game.CanDo(game.LayCardsToBattle));
        Assert.IsTrue(game.CanDo(game.LayCardsToBattle, player1Id));
        Assert.IsFalse(game.CanDo(game.LayCardsToBattle, player2Id));
        {
            Assert.IsTrue(game.LayCardsToBattle(player1Id, Array.Empty<CardToLay>()));
            Assert.IsTrue(game.LayCardsToBattle(player2Id, Array.Empty<CardToLay>()));
        }
        Assert.IsFalse(game.CanDo(game.LayCardsToBattle, player1Id));
        Assert.IsFalse(game.CanDo(game.LayCardsToBattle, player2Id));
        Assert.That(game.Players[0].BattlingDeck.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].BattlingDeck.Count, Is.EqualTo(0));

        // PlayDices
        Assert.IsFalse(game.CanDo(game.PlayDices));
        Assert.IsTrue(game.CanDo(game.PlayDices, player1Id));
        Assert.IsTrue(game.CanDo(game.PlayDices, player2Id));
        {
            Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
            Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        }
        Assert.IsFalse(game.CanDo(game.PlayDices));
        Assert.IsFalse(game.CanDo(game.PlayDices, player1Id));
        Assert.IsFalse(game.CanDo(game.PlayDices, player2Id));
        Assert.That(game.Players[0].FreeDiceOutcomes.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].FreeDiceOutcomes.Count, Is.EqualTo(1));

        // PassReplayDices or ReplayDices
        Assert.IsTrue(game.CanDo(game.PassReplayDices, player1Id));
        Assert.IsTrue(game.CanDo(game.PassReplayDices, player2Id));
        Assert.IsTrue(game.CanDo(game.ReplayDices, player1Id));
        Assert.IsTrue(game.CanDo(game.ReplayDices, player2Id));
        {
            Assert.IsTrue(game.ReplayDices(player1Id, 1, () => new Random(2)));
        }
        Assert.IsFalse(game.CanDo(game.PassReplayDices, player1Id));
        Assert.IsFalse(game.CanDo(game.ReplayDices, player1Id));

        Assert.IsTrue(game.CanDo(game.PassReplayDices, player2Id));
        Assert.IsTrue(game.CanDo(game.ReplayDices, player2Id));
        {
            Assert.IsTrue(game.PassReplayDices(player2Id));
        }
        Assert.IsFalse(game.CanDo(game.PassReplayDices, player2Id));
        Assert.IsFalse(game.CanDo(game.ReplayDices, player2Id));

        // AssignDiceToCard or ConfirmAssignDicesToCards
        Assert.IsFalse(game.CanDo(game.AssignDiceToCard));
        Assert.IsTrue(game.CanDo(game.AssignDiceToCard, player1Id));
        Assert.IsTrue(game.CanDo(game.AssignDiceToCard, player2Id));
        Assert.IsTrue(game.CanDo(game.ConfirmAssignDicesToCards, player1Id));
        Assert.IsTrue(game.CanDo(game.ConfirmAssignDicesToCards, player2Id));
        {
            Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1CardId));
            Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2CardId));
            Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
            Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        }
        Assert.IsFalse(game.CanDo(game.AssignDiceToCard));
        Assert.IsFalse(game.CanDo(game.AssignDiceToCard, player1Id));
        Assert.IsFalse(game.CanDo(game.AssignDiceToCard, player2Id));
        Assert.IsFalse(game.CanDo(game.ConfirmAssignDicesToCards, player1Id));
        Assert.IsFalse(game.CanDo(game.ConfirmAssignDicesToCards, player2Id));
        Assert.That(game.Players[0].FreeDiceOutcomes.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].FreeDiceOutcomes.Count, Is.EqualTo(0));
        Assert.That(game.Players[0].CardAssignments.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].CardAssignments.Count, Is.EqualTo(1));

        // ChooseCardSkill or AssignCardTarget or RemoveCardTarget or ConfirmAll
        Assert.IsTrue(game.CanDo(game.ConfirmAll, player1Id));
        Assert.IsTrue(game.CanDo(game.ConfirmAll, player2Id));
        Assert.IsTrue(game.CanDo(game.ChooseCardSkill, player1Id));
        Assert.IsTrue(game.CanDo(game.ChooseCardSkill, player2Id));
        Assert.IsTrue(game.CanDo(game.AssignCardTarget, player1Id));
        Assert.IsTrue(game.CanDo(game.AssignCardTarget, player2Id));
        Assert.IsTrue(game.CanDo(game.RemoveCardTarget, player1Id));
        Assert.IsTrue(game.CanDo(game.RemoveCardTarget, player2Id));
        {
            Assert.IsTrue(game.AssignCardTarget(player1Id, hero1CardId, hero2CardId));
            Assert.IsTrue(game.CanDo(game.RemoveCardTarget, player1Id));
            Assert.IsTrue(game.AssignCardTarget(player2Id, hero2CardId, hero1CardId));
            Assert.IsTrue(game.CanDo(game.RemoveCardTarget, player2Id));
            Assert.IsTrue(game.ConfirmAll(player1Id));
            Assert.IsTrue(game.ConfirmAll(player2Id));
        }
        Assert.IsFalse(game.CanDo(game.ConfirmAll, player1Id));
        Assert.IsFalse(game.CanDo(game.ConfirmAll, player2Id));
        Assert.IsFalse(game.CanDo(game.ChooseCardSkill, player1Id));
        Assert.IsFalse(game.CanDo(game.ChooseCardSkill, player2Id));
        Assert.IsFalse(game.CanDo(game.AssignCardTarget, player1Id));
        Assert.IsFalse(game.CanDo(game.AssignCardTarget, player2Id));
        Assert.IsFalse(game.CanDo(game.RemoveCardTarget, player1Id));
        Assert.IsFalse(game.CanDo(game.RemoveCardTarget, player2Id));
        Assert.That(game.Players[0].CardAssignments[hero1CardId].TargetCardIds.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].CardAssignments[hero2CardId].TargetCardIds.Count, Is.EqualTo(1));

        // StartRound and PerformMove
        Assert.IsTrue(game.StartRound(random));
        Assert.IsTrue(game.CanDo(game.PerformRound));
        Assert.IsTrue(game.CanDo(game.PerformMove));

        Assert.IsTrue(game.IsRoundOngoing());
        Assert.IsTrue(game.PerformMove(random));
        Assert.That(game.Players[1].HeroCard.Statistics.HP.CalculateValue(), Is.EqualTo(10));

        Assert.IsFalse(game.CanDo(game.PerformRound));

        Assert.IsTrue(game.IsRoundOngoing());
        Assert.IsTrue(game.PerformMove(random));
        Assert.That(game.Players[0].HeroCard.Statistics.HP.CalculateValue(), Is.EqualTo(15));

        Assert.IsFalse(game.IsRoundOngoing());
        Assert.IsFalse(game.CanDo(game.PerformMove));

        // FinishRound
        Assert.IsTrue(game.CanDo(game.FinishRound));
        Assert.IsTrue(game.FinishRound(random));
        Assert.IsFalse(game.CanDo(game.FinishRound));

        Assert.IsFalse(game.IsGameOver());

        // TakeCardsToHand
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand));
        Assert.IsTrue(game.CanDo(game.TakeCardsToHand, player1Id));
        Assert.IsTrue(game.CanDo(game.TakeCardsToHand, player2Id));
        {
            Assert.IsTrue(game.TakeCardsToHand(player1Id, Array.Empty<CardToTake>(), random));
            Assert.IsTrue(game.TakeCardsToHand(player2Id, Array.Empty<CardToTake>(), random));
        }
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand, player1Id));
        Assert.IsFalse(game.CanDo(game.TakeCardsToHand, player2Id));

        // Repeat Process in 2nd Round
        Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        Assert.IsTrue(game.LayCardsToBattle(player1Id, Array.Empty<CardToLay>()));
        Assert.IsTrue(game.LayCardsToBattle(player2Id, Array.Empty<CardToLay>()));
        Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
        Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        Assert.IsTrue(game.PassReplayDices(player1Id));
        Assert.IsTrue(game.PassReplayDices(player2Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1CardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2CardId));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, hero1CardId, hero2CardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, hero2CardId, hero1CardId));
        Assert.IsTrue(game.ConfirmAll(player1Id));
        Assert.IsTrue(game.ConfirmAll(player2Id));
        Assert.IsTrue(game.StartRound(random));

        Assert.IsTrue(game.IsRoundOngoing());
        Assert.IsTrue(game.PerformMove(random));
        Assert.IsTrue(game.Players[1].IsCardDead(hero2Card));

        Assert.IsTrue(game.IsGameOver());
    }
}
