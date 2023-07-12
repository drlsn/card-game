using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class TakeCardsToHandCommandHandler : ICommandHandler<TakeCardsToHandCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    private static object _versionLock;

    public TakeCardsToHandCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }
    public async ValueTask<Result> Handle(TakeCardsToHandCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);
        if (!game.TakeCardsToHand(user.Id, command.CardsToTake))
            return result.Fail();

        game.CalculateLayDownOrderPerPlayer();

        await _publisher.Publish(new CardsTakenToHandEvent(user.Id, game.Id));
        game.IncrementVersion(ref _versionLock);

        return result;
    }
}

public record TakeCardsToHandCommand(string GameId, string PlayerId, CardToTake[] CardsToTake) : ICommand<Result>;

public class TakeCardsToHandCommandValidator : UserRequestValidator<StartBotGameCommand>
{
    public TakeCardsToHandCommandValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) { }
}
