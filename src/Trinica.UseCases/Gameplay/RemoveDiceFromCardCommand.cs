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

public class RemoveDiceFromCardCommandHandler : ICommandHandler<RemoveDiceFromCardCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public RemoveDiceFromCardCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }
    
    public async ValueTask<Result> Handle(RemoveDiceFromCardCommand cmd, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(cmd.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(cmd.GameId), result);

        if (!game.RemoveDiceFromCard(user.Id, new CardId(cmd.CardId)))
            return result.Fail();

        await _gameRepository.Save(game, result);
        await _publisher.PublishEvents(game);

        return result;
    }
}

public record RemoveDiceFromCardCommand(
    string GameId, string PlayerId, string CardId) : ICommand<Result>;

public class RemoveDiceFromCardCommandValidator : AbstractValidator<RemoveDiceFromCardCommand>
{
    public RemoveDiceFromCardCommandValidator()  {}
}
