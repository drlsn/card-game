namespace Trinica.Entities.Gameplay;

public class Dice
{
    public static DiceOption Play(Random random) =>
        DiceOption.Options[random.Next(DiceOption.Options.Length)];
}
