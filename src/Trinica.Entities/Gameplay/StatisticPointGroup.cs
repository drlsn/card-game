namespace Trinica.Entities.Gameplay;

public class StatisticPointGroup
{
    public StatisticPoint Attack { get; }
    public StatisticPoint HP { get; }
    public StatisticPoint Speed { get; }
    public StatisticPoint Power { get; }

    public StatisticPointGroup(
        StatisticPoint attack = null, 
        StatisticPoint hp = null, 
        StatisticPoint speed = null, 
        StatisticPoint power = null)
    {
        Attack = attack ?? new(0);
        HP = hp ?? new(0);
        Speed = speed ?? new(0);
        Power = power ?? new(0);
    }

    public void Modify(StatisticPointGroup statisticPointGroup, string id)
    {
        var attack = statisticPointGroup.Attack.CalculatedValue;
        var hp = statisticPointGroup.HP.CalculatedValue;
        var speed = statisticPointGroup.Speed.CalculatedValue;
        var power = statisticPointGroup.Power.CalculatedValue;

        Attack.Modify(attack, id);
        HP.Modify(hp, id);
        Speed.Modify(speed, id);
        Power.Modify(power, id);
    }

    public void RemoveAll(string id)
    {
        Attack.RemoveAll(id);
        HP.RemoveAll(id);
        Speed.RemoveAll(id);
        Power.RemoveAll(id);
    }
}
