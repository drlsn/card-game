//using Corelibs.Basic.Events;
//using Corelibs.Basic.Functional;
//using FluentAssertions;
//using Trinica.Entities.Gameplay;
//using Trinica.Entities.Gameplay.Events;
//using Trinica.Entities.Users;

//namespace Corelibs.Basic.Tests.Events;

//internal class RoomEventsDispatcherTests
//{
//    [Test]
//    public void AddRoom_Should_Succeed() =>
//        CreateDispatcher()
//            .AddRoom(new GameId("game-id"))
//            .Should().BeTrue();

//    [Test]
//    public void AddRoom_Should_Fail_If_RunTwice_For_SameId() =>
//        CreateDispatcher()
//            .Then(x => x.AddRoom(new GameId("game-id")))
//            .ThenReturn(x => x.AddRoom(new GameId("game-id")))
//            .Should().BeFalse();

//    [Test]
//    public void RemoveRoom_Should_Succeed_If_Exists() =>
//        CreateDispatcher()
//            .Then(x => x.AddRoom(new GameId("game-id")))
//            .ThenReturn(x => x.RemoveRoom(new GameId("game-id")))
//            .Should().BeTrue();

//    [Test]
//    public void RemoveRoom_Should_Fail_If_Not_Exists() =>
//        CreateDispatcher()
//            .ThenReturn(x => x.RemoveRoom(new GameId("game-id")))
//            .Should().BeFalse();

//    [Test]
//    public void Subscribe_Should_Fail_If_Not_Exists() =>
//        CreateDispatcher()
//            .ThenReturn(x => x.Subscribe("game-id", "user-id", async ev => { }))
//            .Should().BeFalse();

//    record GameFinishedOutEvent(string GameId);
//    [Test]
//    public void Dispatch_Should_Succeed_If_Transformer_Added() =>
//        CreateDispatcher()
//            .Then(x => x.AddRoom(new GameId("game-id")))
//            .Then(x => x.AddEventTransformer<GameFinishedEvent, GameFinishedOutEvent>(ev => new(ev.GameId.Value)))
//            .Then(x => x.Subscribe("game-id", "user-id", async ev => {}))
//            .ThenReturn(x => x.Dispatch(new GameFinishedEvent(new("game-id"))))
//            .Should().BeSameAs(Task.FromResult(true));

//    [Test]
//    public void Dispatch_Should_Fail_If_Transformer_Not_Added() =>
//        CreateDispatcher()
//            .Then(x => x.AddRoom(new GameId("game-id")))
//            .Then(x => x.Subscribe("game-id", "user-id", async ev => { }))
//            .ThenReturn(x => x.Dispatch(new GameFinishedEvent(new("game-id"))))
//            .Should().BeSameAs(Task.FromResult(false));

//    [Test]
//    public void Dispatch_Should_Fail_If_Room_Not_Added() =>
//        CreateDispatcher()
//            .Then(x => x.AddRoom(new GameId("game-id")))
//            .Then(x => x.Subscribe("game-id", "user-id", async ev => { }))
//            .ThenReturn(x => x.Dispatch(new GameFinishedEvent(new("game-id"))))
//            .Should().BeSameAs(Task.FromResult(false));

//    private static RoomEventsDispatcher<GameId, UserId, GameEvent> CreateDispatcher() =>
//        new RoomEventsDispatcher<GameId, UserId, GameEvent>(
//            getRoomIdValue: id => id.Value,
//            getUserIdValue: id => id?.Value,
//            toRoomId: value => new(value),
//            toUserId: value => new(value),
//            getRoomId: ev => ev.GameId,
//            getUserId: ev => ev?.PlayerId);
//}
