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

public class Move
{
    public MoveType Type { get; init; }
    public int Damage { get; set; }
    public bool IsFrozen { get; set; }
}

public abstract class Effect
{
    public abstract string Name { get; }

    public RoundInfo RoundInfo { get; }
    public bool Enabled { get; set; } = true;

    public virtual void OnEffectStart(ICombatCard effectOwner) {}
    public virtual void OnRoundStart(ICombatCard effectOwner) {}
    public virtual void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors, Move move) {}
    public virtual void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move) { }
    public virtual void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move) { }
    public virtual void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move) { }
    public virtual void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move) { }
    public virtual void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move) { }
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

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        effectOwner.Statistics.HP.Modify(DamageInflict, Name);
    }
}

public class ElectrificationEffect : Effect, IEffect
{
    public override string Name => "Electrification";
    public bool IsStacking => false;
    public int DamageInflict { get; }

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        if (move.Type is not MoveType.Skill)
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

    public override void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
    {
        if (move.Type is not MoveType.Attack) 
            return;

        actors.MoveActor.Statistics.Attack.Modify(-DamageAbsorption, Name);
    }

    public override void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
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

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        if (move.Type is not MoveType.Attack)
            return;

        _targetShieldEffects = actors.Targets
            .Select(t => t.Effects)
            .Select(effects => effects.OfType<ShieldEffect>())
            .Aggregate((x, y) => x.Concat(y))
            .ToArray();

        _targetShieldEffects.ForEach(e => e.Enabled = false);
    }

    public override void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        if (move.Type is not MoveType.Attack)
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

    public override void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move)
    {
        if (move.Type is not MoveType.Attack)
            return;

        if (target.Effects.OfType<ShieldEffect>().Count() > 0)
            effectOwner.Statistics.Attack.Modify(DamageFactor, Name, isFactor: true);
    }

    public override void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move)
    {
        if (move.Type is not MoveType.Attack)
            return;

        RemoveEffects(target);
    }
}

public class CriticEffect : Effect, IEffect
{
    public override string Name => "Critical Damage";
    public bool IsStacking => false;

    public int DamageFactor { get; } = 2;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        effectOwner.Statistics.Attack.Modify(DamageFactor, Name, isFactor: true);
        effectOwner.Statistics.Power.Modify(DamageFactor, Name, isFactor: true);
    }

    public override void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        RemoveEffects(effectOwner);
    }
}

public class CounterattackEffect : Effect, IEffect
{
    public override string Name => "Counterattack";
    public bool IsStacking => false;

    public override void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
    {
        if (move.Type is not MoveType.Attack)
            return;

        var counterAttackValue = effectOwner.Statistics.Attack.CalculateValue();
        actors.MoveActor.Statistics.HP.Modify(counterAttackValue);
    }
}

public class BlockEffect : Effect, IEffect
{
    public override string Name => "Block";
    public bool IsStacking => false;

    public override void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
    {
        actors.MoveActor.Statistics.Attack.ModifyLate(0, Name, isFactor: true);
        actors.MoveActor.Statistics.Power.ModifyLate(0, Name, isFactor: true);
    }

    public override void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
    {
        RemoveEffects(actors.MoveActor);
    }
}

public class DamageReflectionEffect : Effect, IEffect
{
    public override string Name => "Damage Reflection";
    public bool IsStacking => false;

    public override void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move)
    {
        if (move.Type is not MoveType.Attack)
            return;

        actors.MoveActor.Statistics.HP.Modify(move.Damage);
    }
}

public class StunEffect : Effect, IEffect
{
    public override string Name => "Stun";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        move.IsFrozen = true;
    }
}

public class SilenceEffect : Effect, IEffect
{
    public override string Name => "Silence";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        move.IsFrozen = true;
    }
}

public interface ICombatCard
{
    public StatisticPointGroup Statistics { get; }
    public List<IEffect> Effects { get; }
}
