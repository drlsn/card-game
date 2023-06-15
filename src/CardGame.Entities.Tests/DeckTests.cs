using CardGame.Entities.Decks;
using CardGame.Entities.Users;

namespace CardGame.Entities.Tests
{
    public class DeckTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Deck_Is_Valid_If_30_Cards()
        {
            var deck = new Deck(
                DeckId.New(), UserId.New(), HeroCardId.New(),
                Enumerable.Range(0, 30).Select(i => UnitCardId.New()).ToList(),
                new(), new(), new());

            Assert.IsTrue(deck.IsValid());
        }

        [Test]
        public void Deck_IsNot_Valid_If_Not_30_Cards()
        {
            var userId = UserId.New();
            var heroId = HeroCardId.New();
            var deck = new Deck(userId, heroId);

            Assert.IsFalse(deck.IsValid());
        }
    }
}
