namespace CardGame.Entities.Gameplay;

public class StatisticPoint
{
    public int Value { get; private set; }

    public StatisticPoint(int value)
    {
        Value = value;
    }
}
