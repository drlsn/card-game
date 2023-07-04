namespace Trinica.Entities.Gameplay;

public interface IEffect
{
    string Name { get; }
    RoundInfo RoundInfo { get; }
    bool IsStacking { get; }

    public void RemoveEffects(StatisticPointGroup statistics);
}

public class RoundInfo
{
    public int RoundsTotal { get; }
    public int RoundsPassed { get; }
    public int RoundsLeft => RoundsTotal - RoundsPassed;
}

public abstract class Effect
{
    public abstract string Name { get; }

    public RoundInfo RoundInfo { get; }

    public virtual void OnEffectStart(ICombatCard effectOwner) {}
    public virtual void OnRoundStart(ICombatCard effectOwner) {}
    public virtual void OnDefend(ICombatCard effectOwner, ICombatCard attacker) {}
    public virtual void OnMove(ICombatCard effectOwner, ICombatCard defender, MoveType moveType) { }
    public virtual void OnRoundFinish(ICombatCard effectOwner) {}

    public void RemoveEffects(StatisticPointGroup statistics)
    {
        statistics.Attack.RemoveAll(Name);
        statistics.HP.RemoveAll(Name);
        statistics.Speed.RemoveAll(Name);
        statistics.Power.RemoveAll(Name);
    }
}

public enum MoveType
{
    Attack,
    Skill
}

public class BurningEffect : Effect, IEffect
{
    public override string Name => "Burning";
    public bool IsStacking => false;

    public int DamageInflict { get; }

    public override void OnRoundStart(ICombatCard effectOwner)
    {
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);

        if (RoundInfo.RoundsPassed == 1)
            effectOwner.Statistics.Attack.Modify(0.5, Name, isFactor: true);
    }
}

public class PoisonEffect : Effect, IEffect
{
    public override string Name => "Poison";
    public bool IsStacking => true;

    public int DamageInflict { get; }

    public override void OnRoundFinish(ICombatCard effectOwner)
    {
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
    }
}

public class BleedingEffect : Effect, IEffect
{
    public override string Name => "Bleeding";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    public override void OnMove(ICombatCard effectOwner, ICombatCard defender, MoveType moveType)
    {
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
    }
}

public class ElectrificationEffect : Effect, IEffect
{
    public override string Name => "Electrification";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    public override void OnMove(ICombatCard effectOwner, ICombatCard defender, MoveType moveType)
    {
        if (moveType is not MoveType.Skill)
            return;

        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
        if (RoundInfo.RoundsPassed == 1)
            effectOwner.Statistics.Attack.Modify(0.5, Name, isFactor: true);
    }
}

public class PiercingDamageEffect : Effect, IEffect
{
    public override string Name => "Piercing Damage";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    public override void OnMove(ICombatCard effectOwner, ICombatCard defender, MoveType moveType)
    {
        if (moveType is not MoveType.Attack)
            return;

        //defender.Statistics
    }
}

public interface ICombatCard
{
    public StatisticPointGroup Statistics { get; }
}
