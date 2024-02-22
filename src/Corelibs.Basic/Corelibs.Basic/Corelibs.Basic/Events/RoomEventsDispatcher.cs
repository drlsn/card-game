using Corelibs.Basic.Collections;
using Corelibs.Basic.UseCases.Events;
using System.Collections.Concurrent;

namespace Corelibs.Basic.Events;

public class RoomEventsDispatcher<TRoomId, TUserId, TBaseDomainEvent>(
    Func<TRoomId, string> getRoomIdValue,
    Func<TUserId, string> getUserIdValue,
    Func<string, TRoomId> toRoomId,
    Func<string, TUserId> toUserId,
    Func<TBaseDomainEvent, TRoomId> getRoomId,
    Func<TBaseDomainEvent, TUserId?> getUserId,
    IEventStore<TRoomId, TBaseDomainEvent> eventStore) :
    IRoomSetup<TRoomId>,
    IRoomEventsSubscriber,
    IRoomEventsDispatcher<TBaseDomainEvent>
{   
    private readonly Func<TRoomId, string> _getRoomIdValue = getRoomIdValue;
    private readonly Func<TUserId, string> _getUserIdValue = getUserIdValue;

    private readonly Func<string, TRoomId> _toRoomId = toRoomId;
    private readonly Func<string, TUserId> _toUserId = toUserId;

    private readonly Func<TBaseDomainEvent, TRoomId> _getRoomId = getRoomId;
    private readonly Func<TBaseDomainEvent, TUserId?> _getUserId = getUserId;

    private readonly IEventStore<TRoomId, TBaseDomainEvent> _eventStore = eventStore;

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

    public async Task<bool> Dispatch(IEnumerable<TBaseDomainEvent> events) =>
        (await Task.WhenAll(events.Select(Dispatch).ToArray())).All(x => true);

    public async Task<bool> Subscribe(
        int? lastEventIndex, string roomIdValue, string userIdValue, Func<object, Task> onEvent)
    {
        var roomId = _toRoomId(roomIdValue);
        var userId = _toUserId(userIdValue);

        if (!_rooms.ContainsKey(roomId))
            return false;

        if (lastEventIndex.HasValue)
        {
            _eventStore.LockSave(roomId);
            try
            {
                var events = await _eventStore.GetEvents(roomId, lastEventIndex.Value);
                if (!await Dispatch(events))
                    return false;
            }
            finally
            {
                _eventStore.UnlockSave(roomId);
            }
        }

        _rooms.AddToListValue(roomId, userId);

        if (!_eventHandlers.TryAdd(userIdValue, onEvent))
            return false;

        return true;
    }

    public bool Unsubscribe(string userId)
    {
        return _eventHandlers.Remove(userId, out var _);
    }
}
