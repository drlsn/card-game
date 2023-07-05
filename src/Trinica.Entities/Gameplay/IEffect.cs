using Corelibs.Basic.Collections;
using Corelibs.Basic.Functional;

namespace Trinica.Entities.Gameplay;

public interface IEffect
{
    string Name { get; }
    RoundInfo RoundInfo { get; }
    bool IsStacking { get; }
    bool Enabled { get; set; }

    public void RemoveEffects(StatisticPointGroup statistics);
}

public class RoundInfo
{
    public int RoundsTotal { get; }
    public int RoundsPassed { get; }
    public int RoundsLeft => RoundsTotal - RoundsPassed;
}

public record ReceiveActors(
    ICombatCard MoveActor,
    ICombatCard[] AllEnemies,
    ICombatCard[] AllAllies);

public record MoveActors(
    ICombatCard[] Targets,
    ICombatCard[] AllEnemies,
    ICombatCard[] AllAllies);

public abstract class Effect
{
    public abstract string Name { get; }

    public RoundInfo RoundInfo { get; }
    public bool Enabled { get; set; } = true;

    public virtual void OnEffectStart(ICombatCard effectOwner) {}
    public virtual void OnRoundStart(ICombatCard effectOwner) {}
    public virtual void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors) {}
    public virtual void AfterReceive(ICombatCard effectOwner, ReceiveActors actors) { }
    public virtual void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, MoveType moveType) { }
    public virtual void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, MoveType moveType) { }
    public virtual void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType) { }
    public virtual void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType) { }
    public virtual void OnRoundFinish(ICombatCard effectOwner) {}

    public void RemoveEffects(StatisticPointGroup statistics)
    {
        statistics.Attack.RemoveAll(Name);
        statistics.HP.RemoveAll(Name);
        statistics.Speed.RemoveAll(Name);
        statistics.Power.RemoveAll(Name);
    }

    public void RemoveEffects(ICombatCard combatCard) =>
        RemoveEffects(combatCard.Statistics);

    public void RemoveEffects(IEnumerable<ICombatCard> combatCards) =>
        combatCards.ForEach(RemoveEffects);
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

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
    }
}

public class ElectrificationEffect : Effect, IEffect
{
    public override string Name => "Electrification";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        if (moveType is not MoveType.Skill)
            return;
        
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
        if (RoundInfo.RoundsPassed == 1)
            effectOwner.Statistics.Attack.Modify(0.5, Name, isFactor: true);
    }
}

public class ShieldEffect : Effect, IEffect
{
    public override string Name => "Shield";
    public bool IsStacking => true;

    public int DamageAbsorption { get; } = 300;

    public override void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors)
    {
        actors.MoveActor.Statistics.Attack.Modify(-DamageAbsorption, Name);
    }

    public override void AfterReceive(ICombatCard effectOwner, ReceiveActors actors)
    {
        RemoveEffects(actors.MoveActor.Statistics);
    }
}

public class PiercingDamageEffect : Effect, IEffect
{
    public override string Name => "Piercing Damage";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    private ShieldEffect[] _targetShieldEffects;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        if (moveType is not MoveType.Attack)
            return;

        _targetShieldEffects = actors.Targets
            .Select(t => t.Effects)
            .Select(effects => effects.OfType<ShieldEffect>())
            .Aggregate((x, y) => x.Concat(y))
            .ToArray();

        _targetShieldEffects.ForEach(e => e.Enabled = false);
    }

    public override void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        if (moveType is not MoveType.Attack)
            return;

        _targetShieldEffects.ForEach(e => e.Enabled = true);
        _targetShieldEffects = null;
    }
}

public class CrushingDamageEffect : Effect, IEffect
{
    public override string Name => "Crushing Damage";
    public bool IsStacking => false;
    public int DamageFactor { get; } = 2;

    public override void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, MoveType moveType)
    {
        if (moveType is not MoveType.Attack)
            return;

        if (target.Effects.OfType<ShieldEffect>().Count() > 0)
            effectOwner.Statistics.Attack.Modify(DamageFactor, Name, isFactor: true);
    }

    public override void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, MoveType moveType)
    {
        if (moveType is not MoveType.Attack)
            return;

        RemoveEffects(target);
    }
}

public class CriticEffect : Effect, IEffect
{
    public override string Name => "Critical Damage";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        effectOwner.Statistics.Attack.Modify(2, Name, isFactor: true);
    }

    public override void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, MoveType moveType)
    {
        RemoveEffects(effectOwner);
    }
}

public interface ICombatCard
{
    public StatisticPointGroup Statistics { get; }
    public List<IEffect> Effects { get; }
}
