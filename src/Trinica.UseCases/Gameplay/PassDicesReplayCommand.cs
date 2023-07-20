using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using FluentValidation;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class PassDicesReplayCommandHandler : ICommandHandler<PassDicesReplayCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public PassDicesReplayCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }
    
    public async ValueTask<Result> Handle(PassDicesReplayCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);

        if (!game.PassReplayDices(user.Id))
            return result.Fail();

        await _publisher.Publish(new DicesReplayPassedEvent(game.Id, user.Id));
        await _gameRepository.Save(game, result);

        return result;
    }
}

public record PassDicesReplayCommand(string GameId, string PlayerId) : ICommand<Result>;

public class PassDicesReplayCommandValidator : AbstractValidator<PassDicesReplayCommand>
{
    public PassDicesReplayCommandValidator()  {}
}
