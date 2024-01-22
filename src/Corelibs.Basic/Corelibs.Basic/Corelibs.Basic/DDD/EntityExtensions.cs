namespace Corelibs.Basic.DDD;

public static class EntityExtensions
{
    public static void LockAndIncrementVersion<TId>(this IEntity<TId> game, ref object _lockValue)
        where TId : EntityId
    {
        lock (_lockValue)
        {
            game.Version++; 
        }
    }

    public static void IncrementVersion<TId>(this IEntity<TId> game, ref object _lockValue)
        where TId : EntityId
    {
        game.Version++;
    }
}
