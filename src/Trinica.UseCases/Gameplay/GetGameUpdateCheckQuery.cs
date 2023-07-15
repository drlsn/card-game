using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Mediator;
using Trinica.Entities.Gameplay;

namespace Trinica.UseCases.Gameplay;

public class GetGameUpdateCheckQueryHandler : IQueryHandler<GetGameUpdateCheckQuery, Result<GetGameUpdateCheckQueryResponse>>
{
    private readonly IRepository<Game, GameId> _gameRepository;

    public GetGameUpdateCheckQueryHandler(
        IRepository<Game, GameId> gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async ValueTask<Result<GetGameUpdateCheckQueryResponse>> Handle(GetGameUpdateCheckQuery query, CancellationToken cancellationToken)
    {
        var result = Result<GetGameUpdateCheckQueryResponse>.Success();

        var game = await _gameRepository.Get(new GameId(query.GameId), result);
        
        bool hasToUpdate = game.Version != query.Version;

        return result.With(
            new GetGameUpdateCheckQueryResponse(hasToUpdate));
    }
}

public record GetGameUpdateCheckQuery(
    string GameId,
    uint Version) : IQuery<Result<GetGameUpdateCheckQueryResponse>>;

public record GetGameUpdateCheckQueryResponse(bool hasToUpdate);
