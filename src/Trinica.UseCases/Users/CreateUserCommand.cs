using Corelibs.Basic.Auth;
using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using System.Security.Claims;
using Trinica.Entities.Users;

namespace Trinica.UseCases.Users;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result>
{
    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
    private readonly IRepository<User, UserId> _userRepository;

    public CreateUserCommandHandler(
        IAccessorAsync<ClaimsPrincipal> userAccessor,
        IRepository<User, UserId> userRepository)
    {
        _userAccessor = userAccessor;
        _userRepository = userRepository;
    }

    public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var userId = await _userAccessor.GetUserID<UserId>();
        var user = await _userRepository.Get(userId, result);
        if (user != null)
            return result;

        user = new User(userId);

        await _userRepository.Save(user, result);

        return result;
    }
}

public record CreateUserCommand() : ICommand<Result>;

public class CreateUserValidator : UserRequestValidator<CreateUserCommand>
{
    //public CreateUserValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) {}
}
