using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class TakeCardToHandCommandHandler : ICommandHandler<TakeCardToHandCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public TakeCardToHandCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(TakeCardToHandCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);
        if (!game.TakeCardToHand(user.Id, command.CardToTake.ToCardToTake()))
            return result.Fail();

        await _publisher.Publish(new CardsTakenToHandEvent(user.Id, game.Id));

        if (game.CanDo(game.CalculateLayDownOrderPerPlayer))
        {
            if (game.CalculateLayDownOrderPerPlayer())
                await _publisher.Publish(new LayCardDownOrderCalculatedEvent(game.Id, game.CardsLayOrderPerPlayer.ToArray()));
        }

        await _gameRepository.Save(game, result);

        return result;
    }
}

public record TakeCardToHandCommand(string GameId, string PlayerId, string CardToTake) : ICommand<Result>;

public class TakeCardToHandCommandValidator : UserRequestValidator<TakeCardToHandCommand>
{
    public TakeCardToHandCommandValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) { }
}
