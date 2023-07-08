using Corelibs.Basic.Functional;

namespace Trinica.Entities.Gameplay;

public static class DiceOutcome_To_Element_Converter
{
    public static Element ToElement(this DiceOutcome diceOutcome) =>
        diceOutcome.Match(
            (DiceOutcome.Fire, Element.Fire),
            (DiceOutcome.Ice, Element.Ice),
            (DiceOutcome.Storm, Element.Storm),
            (DiceOutcome.Earth, Element.Earth));
}
