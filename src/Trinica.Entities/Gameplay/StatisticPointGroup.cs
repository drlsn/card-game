namespace Trinica.Entities.Gameplay;

public class StatisticPointGroup
{
    public StatisticPoint Attack { get; init; }
    public StatisticPoint HP { get; init; }
    public StatisticPoint Speed { get; init; }
    public StatisticPoint Power { get; init; }

    public StatisticPointGroup(
        StatisticPoint attack, 
        StatisticPoint hp, 
        StatisticPoint speed, 
        StatisticPoint power)
    {
        Attack = attack;
        HP = hp;
        Speed = speed;
        Power = power;
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
