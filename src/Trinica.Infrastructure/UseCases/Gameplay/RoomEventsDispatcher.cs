using Corelibs.Basic.Collections;
using System.Collections.Concurrent;

namespace Trinica.Infrastructure.UseCases.Gameplay;

public class RoomEventsDispatcher<TRoomId, TUserId, TDomainEvent>(
    Func<TRoomId, string> getRoomIdValue,
    Func<TUserId, string> getUserIdValue,
    Func<string, TRoomId> toRoomId,
    Func<string, TUserId> toUserId,
    Func<TDomainEvent, TRoomId> getRoomId,
    Func<TDomainEvent, TUserId> getUserId) : IEventsDispatcher<TRoomId, TUserId, TDomainEvent>
{
    private readonly Func<TRoomId, string> _getRoomIdValue = getRoomIdValue;
    private readonly Func<TUserId, string> _getUserIdValue = getUserIdValue;

    private readonly Func<string, TRoomId> _toRoomId = toRoomId;
    private readonly Func<string, TUserId> _toUserId = toUserId;

    private readonly Func<TDomainEvent, TRoomId> _getRoomId = getRoomId;
    private readonly Func<TDomainEvent, TUserId> _getUserId = getUserId;

    private readonly ConcurrentDictionary<TRoomId, List<TUserId>> _rooms = new();
    private readonly ConcurrentDictionary<string, Func<object, Task>> _eventHandlers = new();
    private readonly Dictionary<Type, Func<TDomainEvent, object>> _eventTransformers = new();

    public bool AddRoom(TRoomId id) => _rooms.TryAdd(id, new());
    public bool RemoveRoom(TRoomId id) => _rooms.Remove(id, out var _);

    public async Task Dispatch(TDomainEvent @event)
    {
        var eventType = @event.GetType();
        var outgoingEvent = _eventTransformers[eventType].Invoke(@event);

        var roomId = _getRoomId(@event);
        var userId = _getUserId(@event);

        var room = _rooms[roomId];
        var otherUsers = room.Except(userId);

        var userIdValue = _getUserIdValue(userId);
        await Task.WhenAll(
            otherUsers.Select(
                user => _eventHandlers[userIdValue].Invoke(outgoingEvent)));
    }

    public bool Subscribe(string roomIdValue, string userIdValue, Func<object, Task> onEvent)
    {
        var roomId = _toRoomId(roomIdValue);
        var userId = _toUserId(userIdValue);

        _rooms.AddToListValue(roomId, userId);

        return _eventHandlers.TryAdd(roomIdValue, onEvent);
    }

    public bool Unsubscribe(string userId)
    {
        return _eventHandlers.Remove(userId, out var _);
    }
}

public interface IEventsDispatcher<TRoomId, TUserId, TRoomEvent>
{
    bool AddRoom(TRoomId id);
    bool RemoveRoom(TRoomId id);

    Task Dispatch(TRoomEvent @event);

    bool Subscribe(string roomId, string userId, Func<object, Task> onEvent);

    bool Unsubscribe(string userId);
}
