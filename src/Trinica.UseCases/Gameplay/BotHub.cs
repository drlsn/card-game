using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class BotHub : 
    INotificationHandler<CardsTakenToHandEvent>
{
    private readonly Dictionary<GameId, BotGame> _botGames = new();

    private IMediator _mediator;

    public BotHub(IMediator mediator)
    {
        _mediator = mediator;
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

        await _mediator.Send(
            new TakeCardsToHandCommand(game.GameId, game.BotId, 
                Enumerable.Range(0, 8).Select(i => new CardToTake(random.Next(2) == 0 ? CardSource.Own : CardSource.CommonPool)).ToArray()));
    }
}

public record BotGame(UserId BotId, GameId GameId, GameActionController GameActionController);
