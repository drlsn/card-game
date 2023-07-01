using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public record CardToLay(CardId CardId, CardId TargetCardId = null);
