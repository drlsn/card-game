using Corelibs.Basic.Blocks;
using FluentValidation;
using FluentValidation.Results;
using Mediator;

namespace Corelibs.Basic.UseCases;

public class ValidationBehaviour<TCommand, TResult> : IPipelineBehavior<TCommand, TResult>
    where TCommand : IBaseCommand
    where TResult : Result
{
    private readonly IValidator<TCommand> _validator;

    public ValidationBehaviour(IValidator<TCommand> validator)
    {
        _validator = validator;
    }

    public async ValueTask<TResult> Handle(TCommand command, CancellationToken cancellationToken, MessageHandlerDelegate<TCommand, TResult> next)
    {
        var result = await _validator.ValidateAsync(command);
        if (!result.IsValid)
            return result.ToResult() as TResult;

        return await next(command, cancellationToken);
    }
}

public static class ValidationResult_To_CorelibsResult_Converter
{
    public static Result ToResult(this ValidationResult validationResult)
    {
        var result = Result.Success();
        if (validationResult.IsValid)
            return result;

        return result.WithErrors(validationResult.Errors.Select(e => e.ErrorMessage));
    }
}
