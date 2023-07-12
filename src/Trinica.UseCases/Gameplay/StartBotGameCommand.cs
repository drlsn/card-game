using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.HeroCards;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class StartBotGameCommandHandler : ICommandHandler<StartBotGameCommand, Result>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly BotHub _botHub;

    public StartBotGameCommandHandler(
        BotHub botHub,
        IRepository<User, UserId> userRepository,
        IRepository<Game, GameId> gameRepository)
    {
        _botHub = botHub;
        _userRepository = userRepository;
        _gameRepository = gameRepository;
    }

    public async ValueTask<Result> Handle(StartBotGameCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var user = await _userRepository.Get(new UserId(command.PlayerId), result);

        var player = CreatePlayer(user.Id);
        var bot = CreatePlayer();
        var game = new Game(EntityId.New<GameId>(), new[] { player, bot });
        if (!game.StartGame(player.Id))
            return result.Fail();

        if (!game.StartGame(bot.Id))
            return result.Fail();

        if (!game.TakeCardsToCommonPool())
            return result.Fail();

        _botHub.AddGame(user.Id, game.Id, game.ActionController);

        await _gameRepository.Save(game, result);

        user.ChangeLastGame(game.Id);
        await _userRepository.Save(user, result);

        return result;
    }

    public static Player CreatePlayer(UserId userId = null) 
    {
        var statistics = new StatisticPointGroup(
            attack: new(10),
            hp: new(10),
            speed: new(10),
            power: new(10));

        var hero = new HeroCard(EntityId.New<HeroCardId>(), statistics);
        var fieldDeck = new FieldDeck();

        return new Player(userId ?? EntityId.New<UserId>(), EntityId.New<DeckId>(), hero, fieldDeck);
    }
}

public record StartBotGameCommand(string PlayerId) : ICommand<Result>;

public class StartBotGameValidator : UserRequestValidator<StartBotGameCommand>
{
    public StartBotGameValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) { }
}
