namespace Trinica.Entities.Gameplay;

public record DiceOption(string Value)
{
    public static readonly DiceOption Fire = new("fire");
    public static readonly DiceOption Ice = new("ice");
    public static readonly DiceOption Storm = new("storm");
    public static readonly DiceOption Earth = new("earth");
    public static readonly DiceOption Attack = new("attack");

    public static readonly DiceOption[] Options = new DiceOption[] { Fire, Ice, Storm, Earth, Attack, Attack };
}
