using Trinica.UseCases.Gameplay;

namespace Trinica.UI.Common.State;

public class GameState
{
    public bool IsLayingCardToTarget { get; set; }
    public CardDTO LastSelectedCard { get; set; }
}
