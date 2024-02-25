using Corelibs.Basic.Blocks;
using Corelibs.Basic.Collections;
using Corelibs.Basic.Repository;
using Mediator;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Gameplay;

public class GetCurrentGameQueryHandler : IQueryHandler<GetCurrentGameQuery, Result<GetCurrentGameQueryResponse>>
{
    private readonly IRepository<Game, GameId> _gameRepository;
    private readonly IRepository<User, UserId> _userRepository;

    public GetCurrentGameQueryHandler(
        IRepository<Game, GameId> gameRepository, 
        IRepository<User, UserId> userRepository)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<GetCurrentGameQueryResponse>> Handle(GetCurrentGameQuery query, CancellationToken cancellationToken)
    {
        var result = Result<GetCurrentGameQueryResponse>.Success();

        var user = await _userRepository.Get(new UserId(query.PlayerId), result);
        if (!result.ValidateSuccessAndValues())
            return result;

        if (user.LastGameId is null)
            return result.With(new GetCurrentGameQueryResponse());

        var game = await _gameRepository.Get(new GameId(user.LastGameId.Value), result);
        if (!result.ValidateSuccessAndValues())
            return Result<GetCurrentGameQueryResponse>.Success(new GetCurrentGameQueryResponse());

        return result.With(new GetCurrentGameQueryResponse(game.Id.Value));
    }
}

public record GetCurrentGameQuery(
    string PlayerId) : IQuery<Result<GetCurrentGameQueryResponse>>;

public record GetCurrentGameQueryResponse(
    string? GameId = null);