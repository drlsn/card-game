using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using FluentValidation;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class AssignTargetToCardCommandHandler : ICommandHandler<AssignTargetToCardCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public AssignTargetToCardCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }
    
    public async ValueTask<Result> Handle(AssignTargetToCardCommand cmd, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(cmd.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(cmd.GameId), result);

        if (!game.AssignCardTarget(user.Id, new CardId(cmd.CardId), new CardId(cmd.TargetCardId)))
            return result.Fail();

        await _gameRepository.Save(game, result);
        await _publisher.PublishEvents(game);

        return result;
    }
}

public record AssignTargetToCardCommand(
    string GameId, string PlayerId, string CardId, string TargetCardId) : ICommand<Result>;

public class AssignTargetToCardCommandValidator : AbstractValidator<AssignTargetToCardCommand>
{
    public AssignTargetToCardCommandValidator()  {}
}
