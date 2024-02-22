namespace Corelibs.Basic.Events;

public interface IRoomSetup<TRoomId>
{
    bool AddRoom(TRoomId id);
    bool RemoveRoom(TRoomId id);
}

public interface IRoomEventsDispatcher<TRoomEvent>
{
    Task<bool> Dispatch(TRoomEvent @event);
    Task<bool> Dispatch(IEnumerable<TRoomEvent> events);
}

public interface IRoomEventsSubscriber
{

    Task<bool> Subscribe(int? lastEventIndex, string gameId, string userId, Func<object, Task> onEvent);

    bool Unsubscribe(string userId);
}
