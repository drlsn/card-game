using Corelibs.Basic.Blocks;
using Corelibs.Basic.Collections;
using Corelibs.Basic.Repository;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
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

                case LayCardDownOrderCalculatedEvent layCardDownOrderCalculatedEvent:
                    await PostHandle(layCardDownOrderCalculatedEvent, game, mediator, stoppingToken);
                    break;

                case CardsLayPassedByPlayerEvent cardsLayPassedByPlayerEvent:
                    await PostHandle(cardsLayPassedByPlayerEvent, game, mediator, stoppingToken);
                    break;

                case CardsLaidDownEvent cardsLaidDownEvent:
                    await PostHandle(cardsLaidDownEvent, game, mediator, stoppingToken);
                    break;

                case DicesPlayedEvent dicesPlayedEvent:
                    await PostHandle(dicesPlayedEvent, game, mediator, stoppingToken);
                    break;

                case DicesReplayPassedEvent dicesReplayPassedEvent:
                    await PostHandle(dicesReplayPassedEvent, game, mediator, stoppingToken);
                    break;

                case AssignsDicesToCardConfirmedEvent assignsDicesToCardConfirmedEvent:
                    await PostHandle(assignsDicesToCardConfirmedEvent, game, mediator, scope.ServiceProvider, stoppingToken);
                    break;

                case AssignTargetsToCardConfirmedEvent assignTargetsToCardConfirmedEvent:
                    await PostHandle(assignTargetsToCardConfirmedEvent, game, mediator, scope.ServiceProvider, stoppingToken);
                    break;

                default:
                    continue;
            }//todo: remove obsolete games!
        }
    }

    #region Event Handlers

    public ValueTask PostHandle(
        GameStartedEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        Task.Run(() => RunPeriodicTask(async random =>
        {
            var source = random.Next(2) == 0 ? CardSource.Own.Value : CardSource.CommonPool.Value;
            return await mediator.Send(
                new TakeCardToHandCommand(game.GameId.Value, game.BotId.Value, source));
        }, ct));

        return ValueTask.CompletedTask;
    }

    private Task RunLayCardTasks(PlayerData botPlayer, BotGame game, IMediator mediator, CancellationToken ct)
    {
        var handCards = new Queue<CardData>(botPlayer.HandDeck);
        return Task.Run(() => RunPeriodicTask(async random =>
        {
            if (handCards.IsEmpty())
            {
                var result2 = await mediator.Send(
                    new PassLayCardToBattleCommand(game.GameId.Value, game.BotId.Value));

                return Result.Failure();
            }

            var handCard = handCards.Dequeue();
            var result = await mediator.Send(
                new LayCardToBattleCommand(game.GameId.Value, game.BotId.Value, handCard.Id.Value));

            return Result.Success();
        }, ct));
    }

    public ValueTask PostHandle(
        LayCardDownOrderCalculatedEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        if (ev.Players.First().Id != game.BotId)
            return ValueTask.CompletedTask;

        var botPlayer = ev.Players.First(p => p.Id == game.BotId);
        RunLayCardTasks(botPlayer, game, mediator, ct);
        return ValueTask.CompletedTask;
    }

    public ValueTask PostHandle(
        CardsLayPassedByPlayerEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        if (ev.Players.First().Id == game.BotId)
            return ValueTask.CompletedTask;

        var botPlayer = ev.Players.First(p => p.Id == game.BotId);
        RunLayCardTasks(botPlayer, game, mediator, ct);
        return ValueTask.CompletedTask;
    }

    public ValueTask PostHandle(
        CardsLaidDownEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        if (ev.Players.First().Id == game.BotId)
            return ValueTask.CompletedTask;

        if (ev.CanLayMore)
            return ValueTask.CompletedTask;

        var botPlayer = ev.Players.First(p => p.Id == game.BotId);
        RunLayCardTasks(botPlayer, game, mediator, ct);
        return ValueTask.CompletedTask;
    }

    public async ValueTask PostHandle(
        DicesPlayedEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        if (ev.PlayerId == game.BotId)
            return;

        RunPeriodicTask(async random => await mediator.Send(new PlayDicesCommand(game.GameId.Value, game.BotId.Value)), ct);
    }

    public async ValueTask PostHandle(
        DicesReplayPassedEvent ev, BotGame game, IMediator mediator, CancellationToken ct)
    {
        if (ev.PlayerId == game.BotId)
            return;

        RunPeriodicTask(async random => await mediator.Send(new PassDicesReplayCommand(game.GameId.Value, game.BotId.Value)), ct);
    }

    public async ValueTask PostHandle(
        AssignsDicesToCardConfirmedEvent ev, BotGame botGame, IMediator mediator, IServiceProvider services, CancellationToken ct)
    {
        if (ev.PlayerId == botGame.BotId)
            return;

        var result = Result.Success();
        var gameRepository = services.GetRequiredService<IRepository<Game, GameId>>();
        var game = await gameRepository.Get(botGame.GameId, result);
        var botPlayer = game.Players.OfId(botGame.BotId);

        var battlingCards = botPlayer.BattlingDeck
            .GetCards().Prepend(botPlayer.HeroCard)
            .Select((card, i) => new { card, i })
            .Where(c => c.card is UnitCard || c.card is HeroCard)
            .ToArray();

        var diceAttacks = botPlayer.DiceOutcomesToAssign
            .Select((outcome, i) => new { outcome, i })
            .Where(d => d.outcome == DiceOutcome.Attack);

        var attacksPerCard = battlingCards.Zip(diceAttacks, (card, attack) => new { card, attack }).ToQueue();
        RunPeriodicTask(Do, ct, 0, 0);

        async Task<Result> Do(Random random)
        {
            var result = Result.Success();
            if (attacksPerCard.Count > 0)
            {
                var attackPerCard = attacksPerCard.Dequeue();
                result = await mediator.Send(
                    new AssignDiceToCardCommand(botGame.GameId.Value, botGame.BotId.Value, attackPerCard.attack.i, attackPerCard.card.card.Id.Value));
                
                return result;
            }
            else
                await mediator.Send(new ConfirmAssignDicesToCardsCommand(botGame.GameId.Value, botGame.BotId.Value));

            return Result.Failure();
        }
    }

    public async ValueTask PostHandle(
        AssignTargetsToCardConfirmedEvent ev, BotGame game, IMediator mediator, IServiceProvider services, CancellationToken ct)
    {
        if (ev.PlayerId == game.BotId)
            return;

        //RunPeriodicTask(async random => await mediator.Send(new PassDicesReplayCommand(game.GameId.Value, game.BotId.Value)), ct);
    }

    #endregion

    private static async Task<Result> RunPeriodicTask(
        Func<Random, Task<Result>> action, 
        CancellationToken ct,
        int minTime = 500,
        int maxTime = 1500)
    {
        try
        {
            var result = Result.Success();
            var random = new Random();

            while (result.IsSuccess && !ct.IsCancellationRequested)
            {
                var delay = random.Next(minTime, maxTime);
                if (maxTime - minTime > 0)
                    await new PeriodicTimer(TimeSpan.FromMilliseconds(delay)).WaitForNextTickAsync(ct);

                result = await action(random);
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result.Failure();
        }
    }
}
