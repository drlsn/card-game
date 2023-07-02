using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public record CardTarget(
    CardId SourceCardId, 
    CardId TargetCardId);
