using Corelibs.Basic.DDD;
using Trinica.Entities.Gameplay;

namespace Trinica.UseCases.Gameplay;

public static class CommonExtensions
{
    public static void IncrementVersion(this IEntity<GameId> game, ref object _lockValue)
    {
        lock (_lockValue) 
        {
            game.Version++; 
        }
    }
}
