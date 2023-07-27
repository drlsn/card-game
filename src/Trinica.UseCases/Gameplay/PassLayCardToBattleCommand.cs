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

public class PassLayCardToBattleCommandHandler : ICommandHandler<PassLayCardToBattleCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public PassLayCardToBattleCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(PassLayCardToBattleCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);

        if (!game.PassLayCardToBattle(user.Id, CardType_ToString_Converter.ToTypeString))
            return result.Fail();

        await _gameRepository.Save(game, result);
        await _publisher.PublishEvents(game);

        return result;
    }
}

public record PassLayCardToBattleCommand(string GameId, string PlayerId) : ICommand<Result>;

public class PassLayCardToBattleCommandValidator : AbstractValidator<PassLayCardToBattleCommand>
{
    public PassLayCardToBattleCommandValidator()  { }
}
