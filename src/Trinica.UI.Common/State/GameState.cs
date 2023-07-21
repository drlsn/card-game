using Trinica.UseCases.Gameplay;
using static Trinica.UI.Common.Views.Board;

namespace Trinica.UI.Common.State;

public class GameState
{
    public bool IsLayingCardToTarget { get; set; }
    public CardDTO? LastSelectedCard { get; set; }
    public ActionButtonState? LastSelectedActionButton { get; set; }
}

public record ActionButtonState(int index, ActionButtonDTO dto);