namespace Trinica.Entities.Gameplay;

public class Dice
{
    public static DiceOutcome Play(Random random) =>
        DiceOutcome.Outcomes[random.Next(DiceOutcome.Outcomes.Length)];
}

public record DiceOutcome(string Value)
{
    public static readonly DiceOutcome Fire = new("fire");
    public static readonly DiceOutcome Ice = new("ice");
    public static readonly DiceOutcome Storm = new("storm");
    public static readonly DiceOutcome Earth = new("earth");
    public static readonly DiceOutcome Attack = new("attack");

    public static readonly DiceOutcome[] Outcomes = new DiceOutcome[] { Fire, Ice, Storm, Earth, Attack, Attack };

    public bool IsElement() =>
        this != Attack;
}