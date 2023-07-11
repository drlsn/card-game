using Corelibs.Basic.Auth;
using Corelibs.Basic.Blocks;
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
    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public TakeCardsToHandCommandHandler(
        IAccessorAsync<ClaimsPrincipal> userAccessor,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _userAccessor = userAccessor;
        _gameRepository = gameRepository;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(TakeCardsToHandCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var userId = await _userAccessor.GetUserID<UserId>();

        var game = await _gameRepository.Get(new GameId(command.GameId), result);
        if (!game.TakeCardsToHand(userId, command.CardsToTake))
            return result.Fail();

        game.CalculateLayDownOrderPerPlayer();

        await _publisher.Publish(new CardsTakenToHandEvent(userId, game.Id));
        
        return result;
    }
}

public record TakeCardsToHandCommand(string GameId, string PlayerId, CardToTake[] CardsToTake) : ICommand<Result>;

public class TakeCardsToHandCommandValidator : UserRequestValidator<StartBotGameCommand>
{
    public TakeCardsToHandCommandValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) { }
}
