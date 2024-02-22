using Corelibs.Basic.DDD;

namespace Corelibs.Basic.Events;

public interface IEventStore<TRoomId, TEvent>
{
    void LockSave(TRoomId roomId);
    void UnlockSave(TRoomId roomId);

    Task<bool> Save(TRoomId roomId, TEvent @event);

    Task<TEvent[]> GetEvents(TRoomId roomId, int startIndex = int.MaxValue);
}
