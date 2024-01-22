using Corelibs.Basic.Blocks;

namespace Corelibs.Basic.UseCases.Events;

public interface IEventHandler<TEvent>
{
    Task<Result> Handle(TEvent ev);
}