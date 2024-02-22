using Corelibs.Basic.DDD;
using Corelibs.Basic.Events;

namespace Corelibs.Basic.Tests.Events;

internal class InMemoryEventsStoreTests
{
    [Test]
    public async Task GetEvents()
    {
        var store = new InMemoryEventStore<string, IDomainEvent>();
        var gameId = "game-id";

        await store.Save(gameId, new TestEvent("event-id", 0));
        await store.Save(gameId, new TestEvent("event-id", 1));
        await store.Save(gameId, new TestEvent("event-id", 2));

        var events = await store.GetEvents(gameId, 2);

        Assert.IsTrue(events.Length == 1);
        Assert.IsTrue(events[0].Timestamp == 2);
    }

    [Test]
    public async Task GetEvents_IfIndexIsSmaller()
    {
        var store = new InMemoryEventStore<string, IDomainEvent>();
        var gameId = "game-id";

        await store.Save(gameId, new TestEvent("event-id", 0));
        await store.Save(gameId, new TestEvent("event-id", 1));
        await store.Save(gameId, new TestEvent("event-id", 2));

        var events = await store.GetEvents(gameId, 1);

        Assert.IsTrue(events.Length == 2);
        Assert.IsTrue(events[0].Timestamp == 1);
    }

    [Test]
    public async Task Save_IfLocked()
    {
        var store = new InMemoryEventStore<string, IDomainEvent>();
        var gameId = "game-id";

        List<int> values = [];
        var tasks = new List<Task>();
        bool unlockFirst = false;
        tasks.Add(Task.Run(() => 
        {
            store.LockSave(gameId);
            values.Add(1);
            while (!unlockFirst)
            {
                if (unlockFirst)
                {
                    store.UnlockSave(gameId);
                    return;
                }
            }
        }));

        await Task.Delay(1000);
        bool saveExecuted = false;
        tasks.Add(Task.Run(async () =>
        {
            await store.Save(gameId, new TestEvent("event-id", 0));
            saveExecuted = true;
        }));

        await Task.Delay(1000);
        Assert.IsFalse(saveExecuted);

        unlockFirst = true;
        await Task.WhenAll(tasks);

        await Task.Delay(1000);
        Assert.IsTrue(saveExecuted);
    }

    record TestEvent(string Id, long Timestamp) : IDomainEvent;
}
