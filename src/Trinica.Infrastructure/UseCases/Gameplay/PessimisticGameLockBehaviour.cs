using Corelibs.Basic.Blocks;
using Mediator;
using Trinica.UseCases.Gameplay;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class PessimisticGameLockBehaviour<TCommand, TResult> : IPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<Result>, IGameCommand
{
    private readonly static Dictionary<string, Mutex> _mutexes = new();

    public async ValueTask<TResult> Handle(
        TCommand command, CancellationToken cancellationToken, MessageHandlerDelegate<TCommand, TResult> next)
    {
        if (!_mutexes.TryGetValue(command.GameId, out var mutex))
            _mutexes.Add(command.GameId, mutex = new());

        mutex.WaitOne();
        try
        {
            return await next(command, cancellationToken);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
}
