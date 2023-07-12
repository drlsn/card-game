using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class BotHub : 
    INotificationHandler<CardsTakenToHandEvent>
{
    private readonly Dictionary<GameId, BotGame> _botGames = new();

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BotHub(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void AddGame(
        UserId botId, GameId gameId, GameActionController gameActionController)
    {
        _botGames.Add(gameId, new(botId, gameId, gameActionController));
    }

    public async ValueTask Handle(CardsTakenToHandEvent ev, CancellationToken cancellationToken)
    {
        var game = _botGames[ev.GameId];
        var random = new Random();

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();

        await mediator.Send(
            new TakeCardsToHandCommand(game.GameId.Value, game.BotId.Value, 
                Enumerable.Range(0, 8)
                    .Select(i => 
                        new CardToTake(random.Next(2) == 0 ? CardSource.Own : CardSource.CommonPool))
                    .ToArray()));
    }
}

public record BotGame(UserId BotId, GameId GameId, GameActionController GameActionController);
