using Corelibs.Basic.Collections;
using System.Collections.Concurrent;

namespace Corelibs.Basic.Events;

public class RoomEventsDispatcher<TRoomId, TUserId, TBaseDomainEvent>(
    Func<TRoomId, string> getRoomIdValue,
    Func<TUserId, string> getUserIdValue,
    Func<string, TRoomId> toRoomId,
    Func<string, TUserId> toUserId,
    Func<TBaseDomainEvent, TRoomId> getRoomId,
    Func<TBaseDomainEvent, TUserId?> getUserId) : IEventsDispatcher<TRoomId, TUserId, TBaseDomainEvent>
{
    private readonly Func<TRoomId, string> _getRoomIdValue = getRoomIdValue;
    private readonly Func<TUserId, string> _getUserIdValue = getUserIdValue;

    private readonly Func<string, TRoomId> _toRoomId = toRoomId;
    private readonly Func<string, TUserId> _toUserId = toUserId;

    private readonly Func<TBaseDomainEvent, TRoomId> _getRoomId = getRoomId;
    private readonly Func<TBaseDomainEvent, TUserId?> _getUserId = getUserId;

    private readonly ConcurrentDictionary<TRoomId, List<TUserId>> _rooms = new();
    private readonly ConcurrentDictionary<string, Func<object, Task>> _eventHandlers = new();
    private readonly Dictionary<Type, Func<TBaseDomainEvent, object>> _eventTransformers = new();

    public bool AddRoom(TRoomId id) => _rooms.TryAdd(id, new());
    public bool RemoveRoom(TRoomId id) => _rooms.Remove(id, out var _);
    public void AddEventTransformer<TFromEvent, TToEvent>(Func<TBaseDomainEvent, TToEvent> transformer) where TFromEvent : TBaseDomainEvent =>
        _eventTransformers.Add(typeof(TFromEvent), from => transformer(from));

    public async Task<bool> Dispatch(TBaseDomainEvent @event)
    {
        var eventType = @event.GetType();
        if (!_eventTransformers.TryGetValue(eventType, out var transformer))
            return false;

        var outgoingEvent = transformer.Invoke(@event);

        var roomId = _getRoomId(@event);
        var userId = _getUserId(@event);

        if (!_rooms.ContainsKey(roomId))
            return false;

        var room = _rooms[roomId];
        var otherUsers = userId is null ? room : room.Except(userId).ToList();

        await Task.WhenAll(
            otherUsers
                .Select(userId => _eventHandlers[_getUserIdValue(userId)].Invoke(outgoingEvent))
                .ToArray());

        return true;
    }

    public bool Subscribe(string roomIdValue, string userIdValue, Func<object, Task> onEvent)
    {
        var roomId = _toRoomId(roomIdValue);
        var userId = _toUserId(userIdValue);

        if (!_rooms.ContainsKey(roomId))
            return false;

        _rooms.AddToListValue(roomId, userId);

        return _eventHandlers.TryAdd(userIdValue, onEvent);
    }

    public bool Unsubscribe(string userId)
    {
        return _eventHandlers.Remove(userId, out var _);
    }
}
