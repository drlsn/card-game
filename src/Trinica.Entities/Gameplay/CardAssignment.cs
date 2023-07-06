using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public class CardAssignment
{
    public DiceOutcome DiceOutcome { get; set; }
    public int SkillIndex { get; set; } = -1;
    public CardId SourceCardId { get; set; }
    public HashSet<CardId> TargetCardIds { get; set; } = new();
}
