using Corelibs.Basic.Collections;
using System.Collections.Concurrent;

namespace Corelibs.Basic.Events;

public class InMemoryEventStore<TRoomId, TEvent> : IEventStore<TRoomId, TEvent>
{
    private readonly ConcurrentDictionary<TRoomId, List<TEvent>> _eventGroups = new();
    private readonly Dictionary<TRoomId, Mutex> _mutexes = new();

    public void LockSave(TRoomId roomId)
    {
        if (!_mutexes.TryGetValue(roomId, out var mutex))
            _mutexes.Add(roomId, mutex = new());

        mutex.WaitOne();
    }

    public void UnlockSave(TRoomId roomId)
    {
        if (!_mutexes.TryGetValue(roomId, out var mutex))
            return;

        mutex.ReleaseMutex();
        _mutexes.Remove(roomId);
    }

    public async Task<bool> Save(TRoomId roomId, TEvent @event)
    {
        if (_mutexes.TryGetValue(roomId, out var mutex))
            mutex.WaitOne();

        try
        {
            if (!_eventGroups.TryGetValue(roomId, out var events))
            {
                events = new(100);
                _eventGroups[roomId] = events;
            }

            events.Add(@event);
        }
        finally
        {
            mutex?.ReleaseMutex();

        }

        return true;
    }

    public async Task<TEvent[]> GetEvents(TRoomId roomId, int startIndex = int.MaxValue)
    {
        if (!_eventGroups.TryGetValue(roomId, out var events))
            return Array.Empty<TEvent>();

        return events.SkipOrDefault(startIndex).ToArray();
    }
}
