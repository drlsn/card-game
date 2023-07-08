using Corelibs.Basic.CLI;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;
using Trinica.Entities.ItemCards;
using Trinica.Entities.SkillCards;
using Trinica.Entities.SpellCards;
using Trinica.Entities.UnitCards;
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
            Assert.IsFalse(game.LayCardsToBattle(player2Id, Array.Empty<CardToLay>()));
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
        Assert.That(game.Players[0].DiceOutcomesToAssign.Count, Is.EqualTo(1));
        Assert.That(game.Players[1].DiceOutcomesToAssign.Count, Is.EqualTo(1));

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
        Assert.That(game.Players[0].DiceOutcomesToAssign.Count, Is.EqualTo(0));
        Assert.That(game.Players[1].DiceOutcomesToAssign.Count, Is.EqualTo(0));
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
        Assert.That(game.Players[1].HeroCard.Statistics.HP.CalculatedValue, Is.EqualTo(10));

        Assert.IsFalse(game.CanDo(game.PerformRound));

        Assert.IsTrue(game.IsRoundOngoing());
        Assert.IsTrue(game.PerformMove(random));
        Assert.That(game.Players[0].HeroCard.Statistics.HP.CalculatedValue, Is.EqualTo(15));

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
        Assert.IsTrue(game.FinishGame(random));
    }

    [Test]
    public void PlayHeroesAndUnits()
    {
        // Player 1
        var hero1Card = CreateHeroCard("hero-1");
        var unitCards1 = CreateUnitCards().WriteLines();
        var deck1 = new FieldDeck(unitCards1.ToList());
        var deck1Id = new DeckId("deck-1");
        var player1Id = new UserId("player-1");
        var player1 = new Player(player1Id, deck1Id, hero1Card, deck1);

        // Player 2
        var hero2Card = CreateHeroCard("hero-2");
        var unitCards2 = CreateUnitCards().WriteLines();
        var deck2 = new FieldDeck(unitCards2.ToList());
        var deck2Id = new DeckId("deck-2");
        var player2Id = new UserId("player-2");
        var player2 = new Player(player2Id, deck2Id, hero2Card, deck2);

        var random = new Random(1);

        // Game
        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

        var cardsToTake = new[] { 
            new CardToTake(CardSource.Own), new CardToTake(CardSource.Own), new CardToTake(CardSource.CommonPool) };

        var cardsToLay1 = new CardToLay[] { new(unitCards1[0].Id), new(unitCards1[1].Id), new(unitCards2[0].Id) }; 
        var cardsToLay2 = new CardToLay[] { new(unitCards2[2].Id), new(unitCards2[1].Id), new(unitCards1[2].Id) };

        Assert.IsTrue(game.StartGame(player1Id, random));
        Assert.IsTrue(game.StartGame(player2Id, random));
        Assert.IsTrue(game.TakeCardsToCommonPool(random));
        Assert.IsTrue(game.TakeCardsToHand(player1Id, cardsToTake, random));
        Assert.IsTrue(game.TakeCardsToHand(player2Id, cardsToTake, random));
        Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        Assert.IsTrue(game.LayCardsToBattle(player1Id, cardsToLay1));
        Assert.IsTrue(game.LayCardsToBattle(player2Id, cardsToLay2));
        Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
        Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        Assert.IsTrue(game.PassReplayDices(player1Id));
        Assert.IsTrue(game.PassReplayDices(player2Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[0].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[0].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[1].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[1].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[2].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[2].SourceCardId));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, hero1Card.Id, hero2Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player2Id, hero2Card.Id, hero1Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[0].SourceCardId, cardsToLay2[0].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[0].SourceCardId, cardsToLay1[0].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[1].SourceCardId, cardsToLay2[1].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[1].SourceCardId, cardsToLay1[1].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[2].SourceCardId, cardsToLay2[2].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[2].SourceCardId, cardsToLay1[2].SourceCardId));
        Assert.IsTrue(game.ConfirmAll(player1Id));
        Assert.IsTrue(game.ConfirmAll(player2Id));

        Assert.IsTrue(game.StartRound(random));
        {
            int i = 0;
            while (game.IsRoundOngoing() && i < 50)
            {
                Assert.IsTrue(game.PerformMove(random));
                i++;
            }
            Assert.That(i, Is.EqualTo(8));

            foreach (var player in game.Players)
                foreach (var card in player.BattlingDeck.UnitCards)
                    Assert.That(card.Statistics.HP.CalculatedValue, Is.EqualTo(15));
        }
        Assert.IsTrue(game.FinishRound(random));

        Assert.IsTrue(game.TakeCardsToHand(player1Id, Array.Empty<CardToTake>(), random));
        Assert.IsTrue(game.TakeCardsToHand(player2Id, Array.Empty<CardToTake>(), random));
        Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        Assert.IsTrue(game.LayCardsToBattle(player1Id, Array.Empty<CardToLay>()));
        Assert.IsTrue(game.LayCardsToBattle(player2Id, Array.Empty<CardToLay>()));
        Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
        Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        Assert.IsTrue(game.PassReplayDices(player1Id));
        Assert.IsTrue(game.PassReplayDices(player2Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[0].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[0].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[1].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[1].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, cardsToLay1[2].SourceCardId));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, cardsToLay2[2].SourceCardId));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, hero1Card.Id, hero2Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player2Id, hero2Card.Id, hero1Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[0].SourceCardId, cardsToLay2[0].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[0].SourceCardId, cardsToLay1[0].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[1].SourceCardId, cardsToLay2[1].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[1].SourceCardId, cardsToLay1[1].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player1Id, cardsToLay1[2].SourceCardId, cardsToLay2[2].SourceCardId));
        Assert.IsTrue(game.AssignCardTarget(player2Id, cardsToLay2[2].SourceCardId, cardsToLay1[2].SourceCardId));
        Assert.IsTrue(game.ConfirmAll(player1Id));
        Assert.IsTrue(game.ConfirmAll(player2Id));

        Assert.IsTrue(game.StartRound(random));
        {
            int i = 0;
            while (game.IsRoundOngoing() && i < 50)
            {
                Assert.IsTrue(game.PerformMove(random));
                i++;
            }
            Assert.That(i, Is.EqualTo(7));

            foreach (var player in game.Players)
                foreach (var card in player.BattlingDeck.UnitCards)
                    Assert.That(card.Statistics.HP.CalculatedValue, Is.EqualTo(5));

            Assert.IsTrue(game.Players[1].HeroCard is null);
        }
        
        Assert.IsTrue(game.IsGameOver());
    }

    [Test]
    public void PlayHeroesWithItems()
    {
        // Player 1
        var hero1Card = CreateHeroCard("hero-1", speed: 20);
        var itemCard1Id = new ItemCardId("item-1");
        var itemCard1 = CreateItemCard(itemCard1Id, attack: 20);
        var deck1 = new FieldDeck(itemCards: new() { itemCard1 });
        var deck1Id = new DeckId("deck-1");
        var player1Id = new UserId("player-1");
        var player1 = new Player(player1Id, deck1Id, hero1Card, deck1);

        // Player 2
        var hero2Card = CreateHeroCard("hero-2");
        var itemCard2Id = new ItemCardId("item-2");
        var itemCard2 = CreateItemCard(itemCard2Id, attack: 5, hp: 10);
        var deck2 = new FieldDeck(itemCards: new() { itemCard2 });
        var deck2Id = new DeckId("deck-2");
        var player2Id = new UserId("player-2");
        var player2 = new Player(player2Id, deck2Id, hero2Card, deck2);

        var random = new Random(1);

        // Game
        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

        var cardsToTake = new[] { new CardToTake(CardSource.Own) };

        var cardsToLay1 = new CardToLay[] { new(itemCard1Id, hero1Card.Id) };
        var cardsToLay2 = new CardToLay[] { new(itemCard2Id, hero2Card.Id) };

        Assert.IsTrue(game.StartGame(player1Id, random));
        Assert.IsTrue(game.StartGame(player2Id, random));
        Assert.IsTrue(game.TakeCardsToCommonPool(random));
        Assert.IsTrue(game.TakeCardsToHand(player1Id, cardsToTake, random));
        Assert.IsTrue(game.TakeCardsToHand(player2Id, cardsToTake, random));
        Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        Assert.IsTrue(game.LayCardsToBattle(player1Id, cardsToLay1));
        Assert.IsTrue(game.LayCardsToBattle(player2Id, cardsToLay2));
        Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
        Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        Assert.IsTrue(game.PassReplayDices(player1Id));
        Assert.IsTrue(game.PassReplayDices(player2Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2Card.Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, hero1Card.Id, hero2Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player2Id, hero2Card.Id, hero1Card.Id));
        Assert.IsTrue(game.ConfirmAll(player1Id));
        Assert.IsTrue(game.ConfirmAll(player2Id));

        Assert.IsTrue(game.StartRound(random));
        {
            int i = 0;
            while (game.IsRoundOngoing() && i < 50)
            {
                Assert.IsTrue(game.PerformMove(random));
                i++;
            }
            Assert.That(i, Is.EqualTo(1));

            Assert.IsTrue(game.Players[1].HeroCard is null);
        }

        Assert.IsTrue(game.IsGameOver());
    }

    [Test]
    public void PlayHeroesAndNoEffectSpells()
    {
        // Player 1
        var hero1Card = CreateHeroCard("hero-1", speed: 20);
        var spellCard1 = CreateSpellCard("spell-1", attack: 20);
        var deck1 = new FieldDeck(spellCards: new() { spellCard1 });
        var deck1Id = new DeckId("deck-1");
        var player1Id = new UserId("player-1");
        var player1 = new Player(player1Id, deck1Id, hero1Card, deck1);

        // Player 2
        var hero2Card = CreateHeroCard("hero-2");
        var spellCard2 = CreateSpellCard("spell-2", attack: 5);
        var deck2 = new FieldDeck(spellCards: new() { spellCard2 });
        var deck2Id = new DeckId("deck-2");
        var player2Id = new UserId("player-2");
        var player2 = new Player(player2Id, deck2Id, hero2Card, deck2);

        var random = new Random(1);

        // Game
        var gameId = new GameId("game");
        var game = new Game(gameId, new[] { player1, player2 });

        var cardsToTake = new[] { new CardToTake(CardSource.Own) };

        var cardsToLay1 = new CardToLay[] { new(spellCard1.Id, hero1Card.Id) };
        var cardsToLay2 = new CardToLay[] { new(spellCard2.Id, hero2Card.Id) };

        Assert.IsTrue(game.StartGame(player1Id, random));
        Assert.IsTrue(game.StartGame(player2Id, random));
        Assert.IsTrue(game.TakeCardsToCommonPool(random));
        Assert.IsTrue(game.TakeCardsToHand(player1Id, cardsToTake, random));
        Assert.IsTrue(game.TakeCardsToHand(player2Id, cardsToTake, random));
        Assert.IsTrue(game.CalculateLayDownOrderPerPlayer());
        Assert.IsTrue(game.LayCardsToBattle(player1Id, cardsToLay1));
        Assert.IsTrue(game.LayCardsToBattle(player2Id, cardsToLay2));
        Assert.IsTrue(game.PlayDices(player1Id, () => new Random(2)));
        Assert.IsTrue(game.PlayDices(player2Id, () => new Random(2)));
        Assert.IsTrue(game.PassReplayDices(player1Id));
        Assert.IsTrue(game.PassReplayDices(player2Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, hero1Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, hero2Card.Id));
        Assert.IsTrue(game.AssignDiceToCard(player1Id, diceIndex: 0, spellCard1.Id));
        Assert.IsTrue(game.AssignDiceToCard(player2Id, diceIndex: 0, spellCard2.Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player1Id));
        Assert.IsTrue(game.ConfirmAssignDicesToCards(player2Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, hero1Card.Id, hero2Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player2Id, hero2Card.Id, hero1Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player1Id, spellCard1.Id, hero2Card.Id));
        Assert.IsTrue(game.AssignCardTarget(player2Id, spellCard2.Id, hero1Card.Id));
        Assert.IsTrue(game.ConfirmAll(player1Id));
        Assert.IsTrue(game.ConfirmAll(player2Id));

        Assert.IsTrue(game.StartRound(random));
        Assert.IsTrue(game.PerformMove(random));
        Assert.IsTrue(game.Players[1].HeroCard is null);
        Assert.IsTrue(game.IsGameOver());
    }

    [Test]
    public void ShouldNotAssignDiceToNotOwnCard()
    {
        var heroCard = CreateHeroCard("hero");
        var deck = new FieldDeck();
        var deckId = new DeckId("deck");
        var playerId = new UserId("player");
        var player = new Player(playerId, deckId, heroCard, deck);

        player.PlayDices(() => new Random(1));
        Assert.IsFalse(player.AssignDiceToCard(0, new HeroCardId("not-my-card")));
    }

    [Test]
    public void ShouldNotAssignDiceIfNoRolledDices()
    {
        var heroCard = CreateHeroCard("hero");
        var deck = new FieldDeck();
        var deckId = new DeckId("deck");
        var playerId = new UserId("player");
        var player = new Player(playerId, deckId, heroCard, deck);

        Assert.IsFalse(player.AssignDiceToCard(0, heroCard.Id));
    }

    public static HeroCard CreateHeroCard(string id,
        int attack = 10, int hp = 20, int speed = 15, int power = 5)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack),
            hp: new(hp),
            speed: new(speed),
            power: new(power));

        var cardId = new HeroCardId(id);
        return new HeroCard(cardId, stats);
    }

    public static UnitCard CreateUnitCard(int i = -1, 
        int attack = 10, int hp = 25, int speed = 15, int power = 5)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack),
            hp: new(hp),
            speed: new(speed),
            power: new(power));

        var id = i.ToString() + "-" + Guid.NewGuid().ToString();
        var cardId = new UnitCardId($"unit-{id}");
        return new UnitCard(cardId, stats);
    }

    public static Gameplay.Cards.SpellCard CreateSpellCard(SpellCardId id, int attack = 10)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack));

        return new Gameplay.Cards.SpellCard(id, stats, new IEffect[] { });
    }

    public static Gameplay.Cards.SpellCard CreateSpellCard(string id, int attack = 10)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack));

        var cardId = new SpellCardId(id);
        return new Gameplay.Cards.SpellCard(cardId, stats, new IEffect[] { });
    }

    public static Gameplay.Cards.SkillCard CreateSkillCard(int i = -1)
    {
        var id = i != -1 ? i.ToString() : Guid.NewGuid().ToString();
        var cardId = new SkillCardId($"unit-{id}");
        return new Gameplay.Cards.SkillCard(cardId, new IEffect[] { });
    }

    public static Gameplay.Cards.ItemCard CreateItemCard(ItemCardId id,
        int attack = 10, int hp = 0, int speed = 0, int power = 0)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack),
            hp: new(hp),
            speed: new(speed),
            power: new(power));

        return new Gameplay.Cards.ItemCard(id, stats);
    }

    public static Gameplay.Cards.ItemCard CreateItemCard(string id,
        int attack = 10, int hp = 0, int speed = 0, int power = 0)
    {
        var stats = new StatisticPointGroup(
            attack: new(attack),
            hp: new(hp),
            speed: new(speed),
            power: new(power));

        var cardId = new ItemCardId(id);
        return new Gameplay.Cards.ItemCard(cardId, stats);
    }

    public static UnitCard[] CreateUnitCards(int count = 3) =>
        Enumerable.Range(0, count).Select(i => CreateUnitCard(i)).ToArray();
}
