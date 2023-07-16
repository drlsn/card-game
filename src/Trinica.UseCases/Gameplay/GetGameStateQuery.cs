using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Mediator;
using System.Numerics;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class GetGameStateQueryHandler : IQueryHandler<GetGameStateQuery, Result<GetGameStateQueryResponse>>
{
    private readonly IRepository<Game, GameId> _gameRepository;

    public GetGameStateQueryHandler(
        IRepository<Game, GameId> gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async ValueTask<Result<GetGameStateQueryResponse>> Handle(GetGameStateQuery query, CancellationToken cancellationToken)
    {
        var result = Result<GetGameStateQueryResponse>.Success();

        var game = await _gameRepository.Get(new GameId(query.GameId), result);

        var state = game.ActionController.ToDTO();

        var player = game.Players.OfId(new UserId(query.PlayerId));
        var playerDto = player.ToDTO()!;

        var enemyPlayers = game.Players.NotOfId(new UserId(query.PlayerId));
        var enemyPlayersDtos = enemyPlayers.ToDTOs();

        var centerCardDto = game.CenterCard.ToDTO();

        return result.With(
            new GetGameStateQueryResponse(
                game.Id.Value, 
                game.Version, 
                state, 
                playerDto, 
                enemyPlayersDtos!, 
                HasCommonCards: game.CommonPool.Count > 0,
                centerCardDto));
    }
}

public record GetGameStateQuery(
    string GameId,
    string PlayerId) : IQuery<Result<GetGameStateQueryResponse>>;

public record GetGameStateQueryResponse(
    string Id, uint Version,
    GameStateDTO State,
    PlayerDTO Player,
    PlayerDTO[] Enemies,
    bool HasCommonCards,
    CardDTO? CenterCard = null);

public record GameStateDTO(
    string[] ExpectedActionTypes,
    string[]? ExpectedPlayers = null,
    string[]? AlreadyMadeActionsPlayers = null,
    bool MustObeyOrder = false);

public record PlayerDTO(
    string PlayerId, 
    CardDTO Hero, 
    CardDeckDTO BattlingDeck, 
    CardDeckDTO HandDeck,
    bool HasIdleCards);

public record CardDeckDTO(
    CardDTO[] Cards);

public record CardDTO(
    string Id,
    string Name,
    string Race,
    string Class,
    string Fraction,
    string Description,
    string Type,
    CardStatisticsDTO? Statistics = null);

public record CardStatisticsDTO(
    CardStatisticDTO Attack,
    CardStatisticDTO HP,
    CardStatisticDTO Speed,
    CardStatisticDTO Power);

public record CardStatisticDTO(
    int Original,
    int Current);

public static class CardType_ToString_Converter
{
    public static string ToTypeString(this ICard card) =>
        card switch
        {
            HeroCard    => "hero",
            UnitCard    => "unit",
            SkillCard   => "skill",
            ItemCard    => "item",
            SpellCard   => "spell",
            _           => ""
        };
}

public static class Statistics_ToDTO_Converter
{
    public static CardStatisticsDTO ToDTO(this StatisticPointGroup stats) =>
        new(new(stats.Attack.OriginalValue, stats.Attack.CalculatedValue),
            new(stats.HP.OriginalValue, stats.HP.CalculatedValue),
            new(stats.Speed.OriginalValue, stats.Speed.CalculatedValue),
            new(stats.Power.OriginalValue, stats.Power.CalculatedValue));
}

public static class FieldDeck_ToDTO_Converter
{
    public static CardDeckDTO ToDTO(this FieldDeck deck) =>
        new(Cards: deck.GetAllCards().Select(c => c.ToDTO()).ToArray());
}

public static class Card_ToDTO_Converter
{
    public static CardDTO ToDTO(this ICard card)
    {
        if (card is null)
            return null;

        var type = card.ToTypeString();
        if (card is ICardWithStats cardWithStats)
            return new CardDTO(
                card.Id.Value,
                card.Name,
                card.Race.Name,
                card.Class.Name,
                card.Fraction.Name,
                Description: "",
                type, 
                cardWithStats.Statistics.ToDTO());

        return new CardDTO(
            card.Id.Value,
            card.Name,
            card.Race.Name,
            card.Class.Name,
            card.Fraction.Name,
            Description: "",
            type);
    }
}

public static class Player_ToDTO_Converter
{
    public static PlayerDTO? ToDTO(this Player player) => !player ? null :
        new PlayerDTO(
            player.Id.Value,
            Hero: player.HeroCard.ToDTO(),
            BattlingDeck: player.BattlingDeck.ToDTO(),
            HandDeck: player.HandDeck.ToDTO(),
            HasIdleCards: player.IdleDeck.Count > 0);

    public static PlayerDTO?[]? ToDTOs(this IEnumerable<Player> players) =>
        players.Select(p => p.ToDTO()).ToArray();
}

public static class ActionController_ToDTO_Converter
{
    public static GameStateDTO ToDTO(this GameActionController controller) =>
        new(controller.ExpectedAction.Types,
            controller.ExpectedAction.ExpectedPlayers.Select(p => p.Value).ToArray(),
            controller.ExpectedAction.AlreadyMadeActionsPlayers.Select(p => p.Value).ToArray(),
            controller.ExpectedAction.MustObeyOrder);
}
