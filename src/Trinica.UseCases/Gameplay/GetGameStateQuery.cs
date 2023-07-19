using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Mediator;
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
        var isLayCardState = game.ActionController.ActionInfo.GetAction(nameof(Game.LayCardsToBattle)) is not null;

        var state = game.ActionController.ToDTO();

        var player = game.Players.OfId(new UserId(query.PlayerId));
        var playerDto = player.ToDTO()!;

        var enemyPlayers = game.Players.NotOfId(new UserId(query.PlayerId));
        var enemyPlayersDtos = enemyPlayers.ToDTOs(handCardsReversed: true, battlingCardsReversed: isLayCardState);

        var centerCardDto = game.CenterCard?.Card.ToDTO();

        return result.With(
            new GetGameStateQueryResponse(
                game.Id.Value, 
                game.Version, 
                state, 
                playerDto, 
                enemyPlayersDtos!,
                HasCommonCards: game.CommonPool.Count > 0,
                centerCardDto,
                game.CenterCard?.PlayerId?.Value));
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
    CardDTO? CenterCard = null,
    string? CenterCardPlayerId = null);

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
    bool IsReversed,
    string? Name = "",
    string? Race = "",
    string? Class = "",
    string? Fraction = "",
    string? Description = "",
    string? Type = "",
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
    public static CardDeckDTO ToDTO(
        this FieldDeck deck, bool isReversed = false) =>
        new(Cards: deck.GetCards().Select(c => c.ToDTO(isReversed)).ToArray());
}

public static class Card_ToDTO_Converter
{
    public static CardDTO ToDTO(
        this ICard card, bool isReversed = false)
    {
        if (card is null)
            return null;

        if (isReversed)
            return new(
                card.Id.Value,
                IsReversed: true);
        
        var type = card.ToTypeString();
        if (card is ICardWithStats cardWithStats)
            return new(
                card.Id.Value,
                IsReversed: false,
                card.Name,
                card.Race.Name,
                card.Class.Name,
                card.Fraction.Name,
                Description: "",
                type, 
                cardWithStats.Statistics.ToDTO());

        return new(
            card.Id.Value,
            IsReversed: false,
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
    public static PlayerDTO? ToDTO(this Player player,
        bool handCardsReversed = false, bool battlingCardsReversed = false)
    {
        if (!player)
            return null;

        return new PlayerDTO(
            player.Id.Value,
            Hero: player.HeroCard.ToDTO(),
            BattlingDeck: player.BattlingDeck.ToDTO(battlingCardsReversed),
            HandDeck: player.HandDeck.ToDTO(handCardsReversed),
            HasIdleCards: player.IdleDeck.Count > 0);
    }

    public static PlayerDTO?[]? ToDTOs(
        this IEnumerable<Player> players, bool handCardsReversed = false, bool battlingCardsReversed = false) =>
        players.Select(p => p.ToDTO(handCardsReversed, battlingCardsReversed)).ToArray();
}

public static class ActionController_ToDTO_Converter
{
    public static GameStateDTO ToDTO(this IActionController controller) =>
        new(controller.ActionInfo.GetActionNames(),
            controller.ActionInfo.ExpectedPlayers.Select(p => p.Value).ToArray(),
            controller.ActionInfo.Actions
                .Where(a => a.Repeat == ActionRepeat.Single)
                .SelectMany(a => a.AlreadyMadeActionByPlayers)
                .Distinct()
                .Select(p => p.Value)
                .ToArray(),
            controller.ActionInfo.MustObeyOrder);
}
