using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using FluentValidation;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class ConfirmAssignDicesToCardsCommandHandler : ICommandHandler<ConfirmAssignDicesToCardsCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public ConfirmAssignDicesToCardsCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }
    
    public async ValueTask<Result> Handle(ConfirmAssignDicesToCardsCommand cmd, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(cmd.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(cmd.GameId), result);

        if (!game.ConfirmAssignDicesToCards(user.Id))
            return result.Fail();

        await _publisher.Publish(new AssignsDicesToCardConfirmedEvent(game.Id, user.Id));
        await _gameRepository.Save(game, result);

        return result;
    }
}

public record ConfirmAssignDicesToCardsCommand(
    string GameId, string PlayerId) : ICommand<Result>;

public class ConfirmAssignDicesToCardsCommandValidator : AbstractValidator<ConfirmAssignDicesToCardsCommand>
{
    public ConfirmAssignDicesToCardsCommandValidator()  {}
}
