using Trinica.Entities.Gameplay;

namespace Trinica.Entities.Tests;

public class GameActionControllerTests
{
    [Test]
    public void CanDo()
    {
        var controller = new GameActionController(DoSomething);

        Assert.IsTrue(controller.CanDo(DoSomething));
        Assert.IsFalse(controller.CanDo(DoSomethingElse));

        controller.SetNextExpectedAction(DoSomethingElse);

        Assert.IsFalse(controller.CanDo(DoSomething));
        Assert.IsTrue(controller.CanDo(DoSomethingElse));
    }

    private void DoSomething() {}
    private void DoSomethingElse(int x) {}
}
