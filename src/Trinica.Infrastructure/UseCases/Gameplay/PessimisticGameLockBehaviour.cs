using Corelibs.Basic.Blocks;
using Mediator;
using Trinica.UseCases.Gameplay;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class PessimisticGameLockBehaviour<TCommand, TResult> : IPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<Result>
{
    private static Dictionary<string, Mutex> _mutexes = new();

    public async ValueTask<TResult> Handle(
        TCommand command, CancellationToken cancellationToken, MessageHandlerDelegate<TCommand, TResult> next)
    {
        if (command is not IGameCommand gameCommand)
            return await next(command, cancellationToken);

        if (!_mutexes.TryGetValue(gameCommand.GameId, out var mutex))
        {
            mutex = new();
            _mutexes.Add(gameCommand.GameId, mutex);
        }

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
