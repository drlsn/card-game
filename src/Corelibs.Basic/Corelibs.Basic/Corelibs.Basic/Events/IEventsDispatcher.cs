namespace Corelibs.Basic.Events;

public interface IEventsDispatcher<TRoomId, TUserId, TRoomEvent>
{
    bool AddRoom(TRoomId id);
    bool RemoveRoom(TRoomId id);

    Task<bool> Dispatch(TRoomEvent @event);

    bool Subscribe(string roomId, string userId, Func<object, Task> onEvent);

    bool Unsubscribe(string userId);
}
