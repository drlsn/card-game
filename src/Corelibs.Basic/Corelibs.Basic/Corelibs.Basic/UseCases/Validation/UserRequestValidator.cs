using Corelibs.Basic.Auth;
using Corelibs.Basic.Repository;
using FluentValidation;
using System.Security.Claims;

namespace Corelibs.Basic.UseCases;

public abstract class UserRequestValidator<TRequest> : AbstractValidator<TRequest>
{
    public UserRequestValidator() {}
    public UserRequestValidator(
        IAccessorAsync<ClaimsPrincipal> userAccessor)
    {
        RuleFor(person => userAccessor)
            .MustAsync(ValidateUserAccessorAsync).WithMessage("A user must be authorized.");
    }

    private async Task<bool> ValidateUserAccessorAsync(
        IAccessorAsync<ClaimsPrincipal> userAccessor, CancellationToken cancellationToken)
    {
        if (userAccessor is null)
            return true;

        var guidString = await userAccessor.GetUserID();
        return Guid.TryParse(guidString, out _);
    }
}
