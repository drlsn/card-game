using Corelibs.Basic.Blocks;
using Mediator;

namespace Corelibs.Basic.UseCases;

public interface IQueryExecutor
{
    Task<TResponse> Execute<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<Result<TResponse>>;

    Task<TResponse> Execute<TQuery, TResponse>(CancellationToken cancellationToken = default)
        where TQuery : IQuery<Result<TResponse>>, new()
        => Execute<TQuery, TResponse>(new TQuery(), cancellationToken);

    Task<TResponse> Execute<TQuery, TResponse>(string id, CancellationToken cancellationToken = default)
        where TQuery : IQuery<Result<TResponse>>
    {
        var query = (TQuery)Activator.CreateInstance(typeof(TQuery), id);
        return Execute<TQuery, TResponse>(query, cancellationToken);
    }
}
