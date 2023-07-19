using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public record CardToLay(
    CardId SourceCardId, 
    CardId TargetCardId = null,
    bool ToCenter = false);
