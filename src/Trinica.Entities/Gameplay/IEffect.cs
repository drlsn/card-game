using Corelibs.Basic.Collections;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public interface IEffect
{
    string Name { get; }
    RoundInfo RoundInfo { get; }
    bool IsStacking { get; }
    bool Enabled { get; set; }

    public void RemoveEffects(StatisticPointGroup statistics);

    void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings);
    void OnRoundStart(ICombatCard effectOwner, ICombatCard[] allEnemies, ICombatCard[] allAllies, RoundSettings roundSettings);
    void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors, Move move);
    void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move);
    void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move);
    void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move);
    void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move);
    void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move);
    void OnRoundFinish(ICombatCard effectOwner);
    void OnEffectEnd(ICombatCard effectOwner, RoundSettings roundSettings);
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
    public ICombatCard PrioritizedTarget { get; set; } = null;
    public bool MoveEnabled { get; set; } = true;
    public bool AttackEnabled { get; set; } = true;
    public bool ItemsEnabled { get; set; } = true;
    public bool SkillsEnabled { get; set; } = true;
    public bool EffectsEnabled { get; set; } = true;
}

public abstract class Effect
{
    public abstract string Name { get; }

    public string Id => Name + GetHashCode();

    public RoundInfo RoundInfo { get; }
    public bool Enabled { get; set; } = true;

    public virtual void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings) {}
    public virtual void OnRoundStart(ICombatCard effectOwner, ICombatCard[] allEnemies, ICombatCard[] allAllies, RoundSettings roundSettings) { }
    public virtual void BeforeReceive(ICombatCard effectOwner, ReceiveActors actors, Move move) {}
    public virtual void AfterReceive(ICombatCard effectOwner, ReceiveActors actors, Move move) {}
    public virtual void BeforeMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move) {}
    public virtual void AfterMoveAtSingleTarget(ICombatCard effectOwner, ICombatCard target, Move move) {}
    public virtual void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move) {}
    public virtual void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move) {}
    public virtual void OnRoundFinish(ICombatCard effectOwner) {}
    public virtual void OnEffectEnd(ICombatCard effectOwner, RoundSettings roundSettings) {}

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

    public override void OnRoundStart(ICombatCard effectOwner, ICombatCard[] allEnemies, ICombatCard[] allAllies, RoundSettings roundSettings)
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

        if (effectOwner.Effects.Contains<StunEffect>())
            return;

        if (effectOwner.Effects.Contains<DazzleEffect>())
            return;

        var counterAttackValue = effectOwner.Statistics.Attack.CalculatedValue;
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
        move.MoveEnabled = false;
    }
}

public class SilenceEffect : Effect, IEffect
{
    public override string Name => "Silence";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        move.ItemsEnabled = false;
        move.SkillsEnabled = false;
    }
}

public class CleanseEffect : Effect, IEffect
{
    public override string Name => "Cleanse";
    public bool IsStacking => false;

    private readonly Random _random;
    public CleanseEffect(Random random)
    {
        _random = random;
    }

    public override void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        if (effectOwner.Effects.Count == 0)
            return;

        var index = _random.Next(effectOwner.Effects.Count);
        effectOwner.Effects.RemoveAt(index);
    }
}

public class ProvocationEffect : Effect, IEffect
{
    public override string Name => "Provocation";
    public bool IsStacking => false;

    public override void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.PrioritizedToAttackCards.Add(Id, effectOwner.Id);
    }

    public override void OnEffectEnd(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.PrioritizedToAttackCards.Remove(Id);
    }
}

public class HidingEffect : Effect, IEffect
{
    public override string Name => "Hiding";
    public bool IsStacking => false;

    public override void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.NotAllowedAsTargetCards.Add(Id, effectOwner.Id);
    }

    public override void OnEffectEnd(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.NotAllowedAsTargetCards.Remove(Id);
    }
}

public class FreezeEffect : Effect, IEffect
{
    public override string Name => "Freeze";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        move.MoveEnabled = false;
    }
}

public class DazzleEffect : Effect, IEffect
{
    public override string Name => "Dazzle";
    public bool IsStacking => false;

    public override void BeforeMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        move.AttackEnabled = false;
    }
}

public class VampirismEffect : Effect, IEffect
{
    public override string Name => "Vampirism";
    public bool IsStacking => false;

    public override void AfterMoveAtAll(ICombatCard effectOwner, MoveActors actors, Move move)
    {
        effectOwner.Statistics.HP.ModifyClamped(move.Damage);
    }
}

public class HealEffect : Effect, IEffect
{
    public override string Name => "Heal";
    public bool IsStacking => false;

    public override void OnRoundStart(ICombatCard effectOwner, ICombatCard[] allEnemies, ICombatCard[] allAllies, RoundSettings roundSettings)
    {
        effectOwner.Statistics.HP.ModifyClamped(int.MaxValue);
    }
}

public class InviolabilityEffect : Effect, IEffect
{
    public override string Name => "Inviolability";
    public bool IsStacking => false;

    public override void OnEffectStart(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.NotAllowedAsTargetCards.Add(Id, effectOwner.Id);
    }

    public override void OnEffectEnd(ICombatCard effectOwner, RoundSettings roundSettings)
    {
        roundSettings.NotAllowedAsTargetCards.Remove(Id);
    }
}

public interface ICombatCard : ICard
{
    CardId Id { get; }
    StatisticPointGroup Statistics { get; }
    List<IEffect> Effects { get; }

    bool DoesPowerDamage(int skillIndex);
    IEffect[] GetEffects(int skillIndex);
}

public class RoundSettings
{
    public Dictionary<string, CardId> PrioritizedToAttackCards = new();
    public Dictionary<string, CardId> NotAllowedAsTargetCards = new();
}
