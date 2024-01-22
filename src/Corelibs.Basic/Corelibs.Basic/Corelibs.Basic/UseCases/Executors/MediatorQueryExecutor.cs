using Mediator;

namespace Corelibs.Basic.UseCases;

public class MediatorQueryExecutor : IQueryExecutor
{
    private readonly IMediator _mediator;

    public MediatorQueryExecutor(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    async Task<TResponse> IQueryExecutor.Execute<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return result.Get();
    }
}
