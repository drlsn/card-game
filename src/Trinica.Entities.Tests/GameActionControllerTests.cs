using Trinica.Entities.Gameplay;

namespace Trinica.Entities.Tests;

public class GameActionControllerTests
{
    private void DoSomething() {}
    private void DoSomethingElse() {}
    private void DoSomethingElseElse() { }

    [Test]
    public void CanDoAction_IsTrue_IfConstructedWithIt()
    {
        var controller = new GameActionController(DoSomething) as IActionController;
        Assert.IsTrue(controller.CanMakeAction(DoSomething));
    }

    [Test]
    public void CanDoAction_IsFalse_IfNotConstructedWithIt()
    {
        var controller = new GameActionController(DoSomethingElse) as IActionController;
        Assert.IsFalse(controller.CanMakeAction(DoSomething));
    }

    [Test]
    public void SetActionDone_IsSuccess()
    {
        var controller = new GameActionController(DoSomething) as IActionController;
        Assert.IsTrue(controller.SetActionDone(DoSomething).IsSuccess);
    }

    [Test]
    public void SetActionDone_IsNotSuccess_IfTriesSetDifferentActionAsDone()
    {
        var controller = new GameActionController(DoSomething) as IActionController;
        Assert.IsFalse(controller.SetActionDone(DoSomethingElse).IsSuccess);
    }

    [Test]
    public void SetActionExpectedNext_IsSuccess_IfSetPreviousActionDone()
    {
        var controller = new GameActionController(DoSomething) as IActionController;
        Assert.IsTrue(controller
            .SetActionDone(DoSomething)
            .SetActionExpectedNext(DoSomethingElse)
            .IsSuccess
        );
    }

    [Test]
    public void SetActionDone_IsNotSuccess_IfNotUserAction_ButUserProvided()
    {
        var controller = new GameActionController(DoSomething) as IActionController;
        Assert.IsFalse(controller.SetActionDone(DoSomething, new Users.UserId("")).IsSuccess);
    }

    [Test]
    public void SetActionDone_IsNotSuccess_IfUserAction_ButUserNotProvided()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .By(new[] { new Users.UserId("id-1") })
            .IsSuccess);

        Assert.IsFalse(controller.SetActionDone(DoSomething).IsSuccess);
    }

    [Test]
    public void SetActionDone_IsNotSuccess_IfUserAction_AndNonExpectedUserIdProvided()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .By(new[] { new Users.UserId("id-1") })
            .IsSuccess);

        Assert.IsFalse(controller.SetActionDone(DoSomething, new Users.UserId("id-2")).IsSuccess);
    }

    [Test]
    public void SetActionDone_IsSuccess_IfUserAction_AndExpectedUserIdProvided()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .By(new[] { new Users.UserId("id-1") })
            .IsSuccess);

        Assert.IsTrue(controller.SetActionDone(DoSomething, new Users.UserId("id-1")).IsSuccess);
    }

    [Test]
    public void SetActionDone_IsNotSuccess_IfUserAction_AndExpectedUserIdProvidedNotInOrder()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .By(new[] { new Users.UserId("id-2"), new Users.UserId("id-1") }, mustObeyOrder: true)
            .IsSuccess);

        Assert.IsFalse(controller.SetActionDone(DoSomething, new Users.UserId("id-1")).IsSuccess);
    }

    [Test]
    public void SetActionExpectedNext_DoesNotChangeAction_IfCalledMultipleTimes()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .SetActionExpectedNext(DoSomethingElse)
            .IsSuccess);

        Assert.IsNull(controller.ActionInfo.GetAction(nameof(DoSomethingElse)));
    }

    [Test]
    public void SetActionExpectedNextOr_IsSuccess_AndHaveActions_IfCalledMultipleTimes()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .Or(DoSomethingElse)
            .IsSuccess);

        Assert.IsTrue(controller.ActionInfo.HasAction(nameof(DoSomething)));
        Assert.IsTrue(controller.ActionInfo.HasAction(nameof(DoSomethingElse)));
    }

    [Test]
    public void SetActionExpectedNext_DoesNotChangeAction_IfOnlyMultipleRepeatActionDone()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething, ActionRepeat.Multiple)
            .Or(DoSomethingElse)
            .IsSuccess);

        Assert.IsFalse(controller.ActionInfo.GetAction(nameof(DoSomething)).DoneBefore);
        Assert.IsTrue(controller.SetActionDone(DoSomething).IsSuccess);
        Assert.IsTrue(controller.ActionInfo.GetAction(nameof(DoSomething)).DoneBefore);

        Assert.IsTrue(controller.SetActionExpectedNext(DoSomethingElseElse).IsSuccess);
        Assert.IsNull(controller.ActionInfo.GetAction(nameof(DoSomethingElseElse)));
    }

    [Test]
    public void CanMakeAction_IsTrue_After_SetActionDone_MultipleTimes_IsOfMultipleRepeat()
    {
        var controller = new GameActionController() as IActionController;

        var user1 = new Users.UserId("id-1");
        var user2 = new Users.UserId("id-2");
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething, ActionRepeat.Multiple)
            .By(new[] { user2, user1 }, mustObeyOrder: true)
            .IsSuccess);

        Assert.IsFalse(controller.CanMakeAction(DoSomething, user1));
        Assert.IsTrue(controller.CanMakeAction(DoSomething, user2));

        Assert.IsTrue(controller.SetActionDone(DoSomething, user2).IsSuccess);
        Assert.IsTrue(controller.CanMakeAction(DoSomething, user2));
        Assert.IsFalse(controller.CanMakeAction(DoSomething, user1));
    }

    [Test]
    public void SetByUserIds_IsNotSuccess_IfNotSetActionExpectedBefore()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsFalse(controller
            .By(new[] { new Users.UserId("id-1") })
            .IsSuccess);
    }

    [Test]
    public void SetByUserIds_SetsActions_AsUserAction()
    {
        var controller = new GameActionController() as IActionController;
        Assert.IsTrue(controller
            .SetActionExpectedNext(DoSomething)
            .By(new[] { new Users.UserId("id-1") })
            .IsSuccess);

        Assert.IsTrue(controller.IsUserAction());
    }
}
