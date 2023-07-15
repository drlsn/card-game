using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.UseCases.Gameplay;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class BotHubWorker : BackgroundService
{
    private IBotHub _botHub;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BotHubWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        _botHub = scope.ServiceProvider.GetRequiredService<IBotHub>();
        while (await _timer.WaitForNextTickAsync(stoppingToken) && 
            !stoppingToken.IsCancellationRequested)
        {
            if (_botHub.Games.Count > 0)
            {
                Console.WriteLine("Has Games!");
            }

            if (_botHub.Events.Count > 0)
            {
                Console.WriteLine("Has Events!");
            }

            if (!_botHub.Events.TryDequeue(out var ev))
                continue;

            var game = _botHub.Games[ev.GameId];
            switch (ev)
            {
                case CardsTakenToHandEvent cardsTakenToHandEvent:
                    await PostHandle(cardsTakenToHandEvent, game, mediator, stoppingToken);
                    break;

                default:
                    continue;
            }//todo: remove obsolete games!
        }
    }

    #region Event Handlers

    public async ValueTask PostHandle(
        CardsTakenToHandEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        var random = new Random();
        await mediator.Send(
            new TakeCardsToHandCommand(game.GameId.Value, game.BotId.Value,
                Enumerable.Range(0, 8)
                    .Select(i => random.Next(2) == 0 ? CardSource.Own.Value : CardSource.CommonPool.Value)
                    .ToArray()));
    }

    #endregion

}
