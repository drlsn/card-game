using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Mediator;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Users;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, Result<GetUserQueryResponse>>
{
    private readonly IRepository<User, UserId> _userRepository;

    public GetUserQueryHandler(IRepository<User, UserId> userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<Result<GetUserQueryResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var result = Result<GetUserQueryResponse>.Success();

        var user = await _userRepository.Get(new UserId(query.UserId), result);

        return result.With(new GetUserQueryResponse(user.Id.Value, user.Version, user.LastGameId.Value));
    }
}

public record GetUserQuery(
    string UserId) : IQuery<Result<GetUserQueryResponse>>;

public record GetUserQueryResponse(
    string Id,
    uint Version,
    string LastGameId);