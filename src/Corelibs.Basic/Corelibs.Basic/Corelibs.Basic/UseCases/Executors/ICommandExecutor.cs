using Corelibs.Basic.Blocks;
using Corelibs.Basic.Reflection;
using Mediator;

namespace Corelibs.Basic.UseCases;

public interface ICommandExecutor
{
    Task<Result> Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<Result>;

    Task<Result> Execute<TCommand>(CancellationToken cancellationToken = default)
        where TCommand : ICommand<Result>, new()
        => Execute(new TCommand(), cancellationToken);

    Task<Result> Execute<TCommand>(string id, CancellationToken cancellationToken = default)
        where TCommand : ICommand<Result>
    {
        TCommand command;
        try
        {
            command = (TCommand)Activator.CreateInstance(typeof(TCommand), id);
        }
        catch (MissingMethodException)
        {
            command = (TCommand)Activator.CreateInstance(typeof(TCommand));
            command.GetType().GetProperty(id, caseSensitive: false).SetValue(command, id);
        }

        return Execute(command, cancellationToken);
    }
}
