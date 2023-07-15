using Corelibs.Basic.Blocks;
using Corelibs.Basic.Functional;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.UseCases.Gameplay;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class BotHubWorker : BackgroundService
{
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
        var botHub = scope.ServiceProvider.GetRequiredService<IBotHub>();

        while (await _timer.WaitForNextTickAsync(stoppingToken) && 
            !stoppingToken.IsCancellationRequested)
        {
            //if (botHub.Games.Count > 0)
            //{
            //    Console.WriteLine("Has Games!");
            //}

            //if (botHub.Events.Count > 0)
            //{
            //    Console.WriteLine("Has Events!");
            //}

            if (!botHub.Events.TryDequeue(out var ev))
                continue;

            var game = botHub.Games[ev.GameId];
            switch (ev)
            {
                case GameStartedEvent gameStartedEvent:
                    await PostHandle(gameStartedEvent, game, mediator, stoppingToken);
                    break;

                default:
                    continue;
            }//todo: remove obsolete games!
        }
    }

    #region Event Handlers

    public async ValueTask PostHandle(
        GameStartedEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        var random = new Random();
        var delays = Enumerable.Range(0, 6).Select(i => random.Next(3000, 8000)).ToArray();
        Task.Run(Send, ct);

        async Task Send()
        {
            try
            {
                var result = Result.Success();
                while (result.IsSuccess && !ct.IsCancellationRequested)
                {
                    var delay = random.Next(500, 1500);
                    var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(delay));
                    
                    await timer.WaitForNextTickAsync(ct);
                    
                    var source = random.Next(2) == 0 ? CardSource.Own.Value : CardSource.CommonPool.Value;
                    result = await mediator.Send(
                        new TakeCardToHandCommand(game.GameId.Value, game.BotId.Value, source));

                    Console.WriteLine($"Bot made take card. Result - {result.IsSuccess}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine($"Finished Taking Cards for bot - {game.BotId.Value}");
        }
    }

    #endregion
}
