using Corelibs.Basic.Auth;
using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class StartBotGameCommandHandler : ICommandHandler<StartBotGameCommand, Result>
{
    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
    private readonly IPublisher _publisher;
    private readonly BotHub _botHub;

    public StartBotGameCommandHandler(
        IAccessorAsync<ClaimsPrincipal> userAccessor,
        IPublisher publisher)
    {
        _userAccessor = userAccessor;
        _publisher = publisher;
    }

    public async ValueTask<Result> Handle(StartBotGameCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var userId = await _userAccessor.GetUserID<UserId>();

        var player = new Player(userId, EntityId.New<DeckId>(), null, null);
        var bot = new Player(EntityId.New<UserId>(), EntityId.New<DeckId>(), null, null);
        var game = new Game(EntityId.New<GameId>(), new[] { player, bot });
        if (!game.StartGame(player.Id))
            return result.Fail();

        if (!game.StartGame(bot.Id))
            return result.Fail();

        if (!game.TakeCardsToCommonPool())
            return result.Fail();

        await _publisher.Publish(new GameStartedByPlayerEvent(player.Id, game.Id, game.ActionController));
        await _publisher.Publish(new GameStartedByPlayerEvent(bot.Id, game.Id, game.ActionController));

        return result;
    }
}

public record StartBotGameCommand() : ICommand<Result>;

public class CreateUserValidator : UserRequestValidator<StartBotGameCommand>
{
    public CreateUserValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) { }
}
