using Corelibs.Basic.Collections;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public class GameActionController : IActionController
{
    private bool _canSetMore;

    public ActionInfo ActionInfo { get; private set; } = new();

    public GameActionController() {}
    public GameActionController(Delegate @delegate) =>
        SetActionExpectedNext(@delegate.Method.Name);

    public GameActionController(string actionName) =>
        SetActionExpectedNext(actionName);

    public bool IsUserAction() =>
        !ActionInfo.ExpectedPlayers.IsNullOrEmpty() &&
        ActionInfo.Actions.All(a => a.IsUserAction);

    public IChainedOperation SetActionDone(string name, UserId? userId = null)
    {
        _canSetMore = false;

        var requiresUserAction = ActionInfo.RequiresUserAction();
        if (requiresUserAction && userId is null ||
           !requiresUserAction && userId is not null)
            return this.Failure();

        if (!ActionInfo.HasAction(name))
            return this.Failure();

        if (userId is not null && !ActionInfo.HasUser(userId))
            return this.Failure();

        if (!ActionInfo.CanMakeAction(name, userId)) 
            return this.Failure();

        var action = ActionInfo.Actions.First(a => a.Name == name);
        var otherActions = ActionInfo.Actions.Except(action).ToArray();

        action = new(action.Name, action.IsUserAction, DoneBefore: true, 
            action.AlreadyMadeActionByPlayers.Append(userId).ToArray(),
            action.Repeat);

        ActionInfo = new()
        {
            Actions = otherActions.Append(action).ToArray(),
            ExpectedPlayers = ActionInfo.ExpectedPlayers,
            MustObeyOrder = ActionInfo.MustObeyOrder
        };

        return this.Success();
    }

    public IChainedOperation SetActionExpectedNext(string name, ActionRepeat repeat = default)
    {
        if (!ActionInfo.HasDoneActions())
            return this.Success();

        _canSetMore = true;

        ActionInfo = new()
        {
            Actions = new[] { new ActionData(name, IsUserAction: false, DoneBefore: false, Array.Empty<UserId>(), repeat) },
        };

        return this.Success();
    }

    public IChainedOperation Or(string name, ActionRepeat repeat = default)
    {
        if (!_canSetMore)
            return this.Success();

        ActionInfo = new()
        {
            Actions = ActionInfo.Actions.Append(
                   new(name, IsUserAction: false, DoneBefore: false, Array.Empty<UserId>(), repeat)).ToArray(),
        };

        return this.Success();
    }

    public bool CanMakeAction(string name, UserId? user = null) => ActionInfo.CanMakeAction(name, user);

    public IChainedOperation By(UserId[] userIds, bool mustObeyOrder = false)
    {
        if (!ActionInfo.HasActions())
            return this.Failure();

        if (ActionInfo.ExpectedPlayers.Any())
            return this.Success();

        ActionInfo = new()
        {
            Actions = ActionInfo.Actions.Select(a =>
                new ActionData(a.Name, IsUserAction: true, a.DoneBefore, a.AlreadyMadeActionByPlayers, a.Repeat)).ToArray(),

            ExpectedPlayers = userIds,
            MustObeyOrder = mustObeyOrder,
        };

        return this.Success();
    }
}

public interface IActionController
{
    #region By Name String

    IChainedOperation SetActionDone(string name, UserId? userId = null);
    IChainedOperation SetActionExpectedNext(string name, ActionRepeat repeat = default);
    IChainedOperation Or(string name, ActionRepeat repeat = default);
    bool CanMakeAction(string name, UserId? user = null);

    #endregion

    #region By Name Delegate

    IChainedOperation SetActionDone(Delegate @delegate, UserId? userId = null) => SetActionDone(@delegate.Method.Name, userId);
    IChainedOperation SetActionExpectedNext(Delegate @delegate, ActionRepeat repeat = default) => SetActionExpectedNext(@delegate.Method.Name, repeat);
    IChainedOperation Or(Delegate @delegate, ActionRepeat repeat = default) => Or(@delegate.Method.Name, repeat);
    bool CanMakeAction(Delegate @delegate, UserId? user = null) => CanMakeAction(@delegate.Method.Name, user);

    #endregion

    #region Other

    IChainedOperation By(UserId[] userIds, bool mustObeyOrder = false);
    bool IsUserAction();

    ActionInfo ActionInfo { get; }

    #endregion
}

public interface IChainedOperation : IActionController
{
    bool IsSuccess { get; }
}

public class ChainedOperation : IChainedOperation
{
    private readonly IActionController _controller;

    public ChainedOperation(
        IActionController controller, bool isSuccess)
    {
        _controller = controller;
        IsSuccess = isSuccess;
    }

    public static void Create(IActionController controller, bool isSuccess) =>
        new ChainedOperation(controller, isSuccess);

    public bool IsSuccess { get; }

    public IChainedOperation SetActionDone(string name, UserId? userId = null)
    {
        if (!IsSuccess)
            ;// return this;

        return _controller.SetActionDone(name, userId);
    }

    public IChainedOperation SetActionExpectedNext(string name, ActionRepeat repeat = default)
    {
        if (!IsSuccess)
            ;// return this;

        return _controller.SetActionExpectedNext(name, repeat);
    }

    public bool CanMakeAction(string name, UserId? user = null) =>
        _controller.CanMakeAction(name, user);

    public IChainedOperation By(UserId[] userIds, bool mustObeyOrder = false)
    {
        if (!IsSuccess)
            ;// return this;

        return _controller.By(userIds, mustObeyOrder);
    }

    public IChainedOperation Or(string name, ActionRepeat repeat = ActionRepeat.Single)
    {
        if (!IsSuccess)
            ;// return this;

        return _controller.Or(name, repeat);
    }

    public bool IsUserAction() => _controller.IsUserAction();

    public ActionInfo ActionInfo => _controller.ActionInfo;
}

public static class ChainedOperationExtensions
{
    public static IChainedOperation Success(this IActionController controller) =>
        new ChainedOperation(controller, true);

    public static IChainedOperation Failure(this IActionController controller) =>
        new ChainedOperation(controller, false);
}

public enum ActionRepeat
{
    Single,
    Multiple
}

public class ActionInfo
{
    public ActionData[] Actions { get; init; } = Array.Empty<ActionData>();
    public bool MustObeyOrder { get; init; }
    public UserId[] ExpectedPlayers { get; init; } = Array.Empty<UserId>();

    public bool CanMakeAction(string name, UserId? userId = null)
    {
        if (name.IsNullOrEmpty())
            return false;

        if (userId is null && !ExpectedPlayers.IsNullOrEmpty())
            return false;

        if (userId is not null && ExpectedPlayers.IsNullOrEmpty())
            return false;

        // Check if user done any single time action before, which forbids to make any more
        var singleTimeActions = Actions.Where(a => a.Repeat is ActionRepeat.Single).ToArray();
        if (singleTimeActions.Any(a => !a.IsUserAction && a.DoneBefore) ||
            singleTimeActions.Any(a => a.IsUserAction && a.AlreadyMadeActionByPlayers.Contains(userId)))
            return false;

        // Check if given action is even expected 
        var action = Actions.FirstOrDefault(a => a.Name == name);
        if (!name.IsNullOrEmpty() && action is null)
            return false;

        if (MustObeyOrder)
        {
            int expectedOrderIndex = Array.FindIndex(ExpectedPlayers, id => id == userId);
            if (action.AlreadyMadeActionByPlayers.Length == expectedOrderIndex)
                return true;
        }
        else
        {
            if (action.Repeat is ActionRepeat.Multiple)
                return true;

            if (!action.AlreadyMadeActionByPlayers.Contains(userId))
                return true;
        }

        return false;
    }

    public bool HasDoneActions()
    {
        if (Actions.IsNullOrEmpty())
            return true;

        var singleTimeActions = Actions.Where(a => a.Repeat is ActionRepeat.Single).ToArray();
        if (ExpectedPlayers.IsNullOrEmpty())
        {
            if (!singleTimeActions.Any(a => a.DoneBefore))
                return false;
        }
        else
        {
            var alreadyMadeActionPlayers = singleTimeActions
                .SelectMany(a => a.AlreadyMadeActionByPlayers)
                .Distinct()
                .ToArray();

            if (!alreadyMadeActionPlayers.HasSameElements(ExpectedPlayers))
                return false;
        }

        return true;
    }

    public bool HasActions() =>
       !Actions.IsNullOrEmpty();

    public bool HasAction(string name) =>
        Actions.Any(a => a.Name == name);

    public bool HasUser(UserId userId) =>
        ExpectedPlayers.Contains(userId);

    public bool RequiresUserAction() => ExpectedPlayers.Any();

    public ActionData GetAction(string name) =>
        Actions.FirstOrDefault(a => a.Name == name);

    public string[] GetActionNames() =>
        Actions.Select(a => a.Name).ToArray();
}

public record ActionData(
    string Name,
    bool IsUserAction,
    bool DoneBefore,
    UserId[] AlreadyMadeActionByPlayers,
    ActionRepeat Repeat = default);
