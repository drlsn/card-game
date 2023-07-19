using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using FluentValidation;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class PlayDicesCommandHandler : ICommandHandler<PlayDicesCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public PlayDicesCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(PlayDicesCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);

        if (!game.PlayDices(user.Id))
            return result.Fail();

        await _publisher.Publish(new DicesPlayedEvent(game.Id, user.Id));
        await _gameRepository.Save(game, result);

        return result;
    }
}

public record PlayDicesCommand(string GameId, string PlayerId) : ICommand<Result>;

public class PlayDicesCommandValidator : AbstractValidator<PlayDicesCommand>
{
    public PlayDicesCommandValidator()  {}
}
