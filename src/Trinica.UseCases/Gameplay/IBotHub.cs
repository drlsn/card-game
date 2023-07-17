using System.Collections.Concurrent;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public interface IBotHub
{
    Dictionary<GameId, BotGame> Games { get; }
    ConcurrentQueue<GameEvent> Events { get; }

    Task AddGame(
        UserId botId, GameId gameId, IActionController actionController);
}

public record BotGame(UserId BotId, GameId GameId, IActionController GameActionController);
