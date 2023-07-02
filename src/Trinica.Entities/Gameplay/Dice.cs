using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public class Dice
{
    public static DiceOption Play(Random random) =>
        DiceOption.Options[random.Next(DiceOption.Options.Length)];
}

public record DiceOption(string Value)
{
    public static readonly DiceOption Fire = new("fire");
    public static readonly DiceOption Ice = new("ice");
    public static readonly DiceOption Storm = new("storm");
    public static readonly DiceOption Earth = new("earth");
    public static readonly DiceOption Attack = new("attack");

    public static readonly DiceOption[] Options = new DiceOption[] { Fire, Ice, Storm, Earth, Attack, Attack };
}

public record DiceOptionIndexPerCard(int DiceIndex, CardId CardId);
public record DiceOptionPerCard(DiceOption Option, CardId CardId = null);
