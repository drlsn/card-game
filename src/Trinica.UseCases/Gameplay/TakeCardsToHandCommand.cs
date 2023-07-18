using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using FluentValidation;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class TakeCardsToHandCommandHandler : ICommandHandler<TakeCardsToHandCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

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
        if (!game.TakeCardsToHand(user.Id, command.CardsToTake.ToCardsToTake()))
            return result.Fail();

        await _publisher.Publish(new CardsTakenToHandEvent(user.Id, game.Id));

        game.CalculateLayDownOrderPerPlayer();
        if (game.CanDo(game.CalculateLayDownOrderPerPlayer))
        {
            if (game.CalculateLayDownOrderPerPlayer())
                await _publisher.Publish(new LayCardDownOrderCalculatedEvent(
                    game.Id, game.Players.Select(p => new PlayerData(
                        p.Id,
                        p.HandDeck.GetCards().Select(c => new CardData(c.Id, c.ToTypeString())).ToArray(),
                        p.BattlingDeck.GetCards().Select(c => new CardData(c.Id, c.ToTypeString())).ToArray())).ToArray()));
        }

        await _gameRepository.Save(game, result);

        return result;
    }
}

public record TakeCardsToHandCommand(string GameId, string PlayerId, string[] CardsToTake) : ICommand<Result>;

public class TakeCardsToHandCommandValidator : AbstractValidator<TakeCardsToHandCommand>
{
    public TakeCardsToHandCommandValidator() {}
}
