using Corelibs.Basic.Collections;

namespace Trinica.Entities.Gameplay;

public class StatisticPoint
{
    public int OriginalValue { get; }
    public List<StatisticPointModifier> Modifiers { get; private set; } = new();
    public List<StatisticPointModifier> ModifiersLate { get; private set; } = new();

    public StatisticPoint(int value)
    {
        OriginalValue = value;
    }

    public int CalculateValue()
    {
        var value = OriginalValue;

        Modifiers.ForEach(m => 
            value = m.IsFactor ? value = (int) (value * m.Value) : (int) (value + m.Value)
        );

        ModifiersLate.ForEach(m =>
            value = m.IsFactor ? value = (int)(value * m.Value) : (int)(value + m.Value)
        );

        return value;
    }

    public void Modify(double value, string id, bool isFactor = false) =>
        Modifiers.Add(new(value, isFactor, id));

    public void ModifyLate(double value, string id, bool isFactor = false) =>
        ModifiersLate.Add(new(value, isFactor, id));

    public void RemoveAll(string id)
    {
        Modifiers.RemoveIf(v => v.Id == id);
        ModifiersLate.RemoveIf(v => v.Id == id);
    }
}

public record StatisticPointModifier(double Value, bool IsFactor = false, string Id = null);