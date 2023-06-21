using CardGame.Entities.Users;
using Corelibs.Basic.DDD;
using System.Text.Json.Serialization;

namespace CardGame.Entities.Decks;

public record HeroCardId(string Value) : EntityId(Value);
public record UnitCardId(string Value) : EntityId(Value);
public record SkillCardId(string Value) : EntityId(Value);
public record ItemCardId(string Value) : EntityId(Value);
public record SpellCardId(string Value) : EntityId(Value);

public record DeckId(string Value) : EntityId(Value);

public class Deck : Entity<DeckId>
{
    public const int RequiredCardCount = 30;

    public UserId UserId { get; private set; }
    public HeroCardId HeroCardId { get; private set; }
    public List<UnitCardId> UnitCardIds { get; private set; }
    public List<SkillCardId> SkillCardIds { get; private set; }
    public List<ItemCardId> ItemCardIds { get; private set; }
    public List<SpellCardId> SpellCardIds { get; private set; }

    [JsonConstructor]
    public Deck(
        DeckId id,
        UserId userId,
        HeroCardId heroCardId,
        List<UnitCardId> unitCardIds,
        List<SkillCardId> skillCardIds,
        List<ItemCardId> itemCardIds,
        List<SpellCardId> spellCardIds) : base(id)
    {
        UserId = userId;
        HeroCardId = heroCardId;
        UnitCardIds = unitCardIds;
        SkillCardIds = skillCardIds;
        ItemCardIds = itemCardIds;
        SpellCardIds = spellCardIds;
    }

    public Deck(UserId userId) => UserId = userId;
    public Deck(UserId userId, HeroCardId heroCardId) : this(userId) => HeroCardId = heroCardId;

    public bool IsValid() =>
        UserId.IsValid() is true &&
        HeroCardId?.IsValid() is true &&
        UnitCardIds?.Count +
        SkillCardIds?.Count +
        ItemCardIds?.Count +
        SpellCardIds?.Count == RequiredCardCount;
}
