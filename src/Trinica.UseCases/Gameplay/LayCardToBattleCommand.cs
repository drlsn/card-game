using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using FluentValidation;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class LayCardToBattleCommandHandler : ICommandHandler<LayCardToBattleCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IPublisher _publisher;

    public LayCardToBattleCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository,
        IPublisher publisher)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(LayCardToBattleCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);
        var game = await _gameRepository.Get(new GameId(command.GameId), result);

        var cardToLay = new CardToLay(new CardId(command.CardId), new CardId(command.TargetCardId), command.ToCenter);
        if (!game.LayCardToBattle(user.Id, cardToLay))
            return result.Fail();

        var canStillLayCardDown = game.CanLayCardDown(user.Id);
        await _publisher.Publish(new CardsLaidDownEvent(game.Id, user.Id, new[] { cardToLay }, canStillLayCardDown, 
            game.Players.ToPlayerData(CardType_ToString_Converter.ToTypeString)));

        await _gameRepository.Save(game, result);

        return result;
    }
}

public record LayCardToBattleCommand(string GameId, string PlayerId, string CardId, string? TargetCardId = null, bool ToCenter = false) : ICommand<Result>;

public class LayCardToBattleCommandValidator : AbstractValidator<LayCardToBattleCommand>
{
    public LayCardToBattleCommandValidator()  { }
}
