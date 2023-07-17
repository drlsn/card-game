using Corelibs.Basic.Collections;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public class GameActionController : IActionController
{
    public Action ExpectedAction = new();

    public GameActionController() {}
    public GameActionController(Delegate @delegate, ActionRepeat repeat = default)
    {
        SetNextExpectedAction(@delegate.Method.Name);
        SetActionExpectedNext(@delegate.Method.Name, repeat);
    }
    public GameActionController(Delegate @delegate, UserId[] expectedPlayers) => SetNextExpectedAction(@delegate.Method.Name, expectedPlayers);
    public GameActionController(string name, ActionRepeat repeat = default)
    {
        SetNextExpectedAction(name);
        SetActionExpectedNext(name, repeat);
    }

    public bool CanDo(Delegate @delegate, UserId userId = null) =>
        CanDo(@delegate.Method.Name, userId);

    public bool CanDo(string actionType, UserId userId = null)
    {
        if (!ExpectedAction.Types.Contains(actionType))
            return false;

        if (ExpectedAction.ExpectsUserAction())
        {
            if (!ExpectedAction.ExpectedPlayers.Contains(userId))
                return false;

            return ExpectedAction.CanMakeAction(userId, actionType);
        }

        return true;
    }

    public bool SetPlayerDoneOrNextExpectedAction(UserId userId, params Delegate[] @delegates) =>
        SetPlayerDoneOrNextExpectedAction(userId, @delegates.Select(d => d.Method.Name).ToArray());

    public bool SetPlayerDoneOrNextExpectedAction(UserId userId, UserId[] expectedPlayers, params Delegate[] @delegates) =>
        SetPlayerDoneOrNextExpectedAction(userId, @delegates.Select(d => d.Method.Name).ToArray(), expectedPlayers);

    public bool SetPlayerDoneOrNextExpectedAction(UserId userId, UserId[] expectedPlayers, bool mustObeyOrder = false, params Delegate[] @delegates) =>
        SetPlayerDoneOrNextExpectedAction(userId, @delegates.Select(d => d.Method.Name).ToArray(), expectedPlayers, mustObeyOrder);

    public bool SetPlayerDoneOrNextExpectedAction(UserId userId, Delegate[] @delegates, UserId[] expectedPlayers = null, bool mustObeyOrder = false) =>
        SetPlayerDoneOrNextExpectedAction(userId, @delegates.Select(d => d.Method.Name).ToArray(), expectedPlayers, mustObeyOrder);

    public bool SetPlayerDoneOrNextExpectedAction(UserId userId, Delegate @delegate, UserId[] expectedPlayers = null, bool mustObeyOrder = false) =>
        SetPlayerDoneOrNextExpectedAction(userId, @delegate.Method.Name, expectedPlayers, mustObeyOrder);

    public bool SetNextExpectedAction(
        UserId[] expectedPlayers,
        params Delegate[] @delegates) => SetNextExpectedAction(@delegates.Select(d => d.Method.Name).ToArray(), expectedPlayers);

    public bool SetNextExpectedAction(
        UserId[] expectedPlayers,
        bool mustObeyOrder,
        params Delegate[] @delegates) => SetNextExpectedAction(@delegates.Select(d => d.Method.Name).ToArray(), expectedPlayers, mustObeyOrder);

    public bool SetNextExpectedAction(params Delegate[] @delegates) => SetNextExpectedAction(@delegates.Select(d => d.Method.Name).ToArray());
    public bool SetNextExpectedAction(
        Delegate @delegate,
        UserId[] expectedPlayers) => SetNextExpectedAction(@delegate.Method.Name, expectedPlayers);
    public bool SetNextExpectedAction(
        Delegate @delegate,
        UserId[] expectedPlayers,
        bool mustObeyOrder) => SetNextExpectedAction(@delegate.Method.Name, expectedPlayers, mustObeyOrder);

    public bool SetNextExpectedAction(
        string type,
        UserId[] expectedPlayers = null,
        bool mustObeyOrder = false) => SetNextExpectedAction(new[] { type }, expectedPlayers, mustObeyOrder);

    public bool SetNextExpectedAction(
        string[] types,
        UserId[] expectedPlayers = null,
        bool mustObeyOrder = false)
    {
        if (!ExpectedAction.HasUsersDoneActions())
            return false;

        ExpectedAction = new()
        {
            Types = types,
            ExpectedPlayers = expectedPlayers ?? Array.Empty<UserId>(),
            MustObeyOrder = mustObeyOrder
        };

        return true;
    }

    public bool SetPlayerDoneOrNextExpectedAction(
        UserId userId,
        string type,
        UserId[] expectedPlayers = null,
        bool mustObeyOrder = false) => SetPlayerDoneOrNextExpectedAction(userId, new[] { type }, expectedPlayers, mustObeyOrder);

    public bool SetPlayerDoneOrNextExpectedAction(
        UserId userId,
        string[] types,
        UserId[] expectedPlayers = null,
        bool mustObeyOrder = false)
    {
        if (!ExpectedAction.HasUsersDoneActions())
        {
            if (ExpectedAction.CanMakeAction(userId))
                ExpectedAction.AlreadyMadeActionsPlayers.Add(userId);
            else
                return false;
        }

        if (!ExpectedAction.HasUsersDoneActions())
            return true;

        ExpectedAction = new()
        {
            Types = types,
            ExpectedPlayers = expectedPlayers ?? Array.Empty<UserId>(),
            MustObeyOrder = mustObeyOrder
        };

        return true;
    }

    #region New Actions

    public ActionInfo ActionInfo { get; private set; } = new();

    public bool IsUserAction() =>
        !ActionInfo.ExpectedPlayers.IsNullOrEmpty() &&
        ActionInfo.Actions.All(a => a.IsUserAction);

    public IChainedOperation SetActionDone(string name, UserId? userId = null)
    {
        var requiresUserAction = ActionInfo.RequiresUserAction();
        if (requiresUserAction && userId is null ||
           !requiresUserAction && userId is not null)
            return this.Failure();

        if (!ActionInfo.HasAction(name))
            return this.Failure();

        if (userId is not null && !ActionInfo.HasUser(userId))
            return this.Failure();

        if (!ActionInfo.CanMakeAction(userId, name)) 
            return this.Failure();

        var action = ActionInfo.Actions.First(a => a.Name == name);
        var otherActions = ActionInfo.Actions.Except(action).ToArray();

        action = new(action.Name, action.IsUserAction, DoneBefore: true, action.AlreadyMadeActionByPlayers, action.Repeat);

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
        if (ActionInfo.RequiresUserAction())
            return this.Failure();

        if (!ActionInfo.HasDoneActions())
            return this.Failure();

        ActionInfo = new()
        {
            Actions = ActionInfo.Actions.Append(
                   new(name, IsUserAction: false, DoneBefore: false, Array.Empty<UserId>(), repeat)).ToArray(),
        };

        return this.Success();
    }

    public IChainedOperation Or(string name, ActionRepeat repeat = default)
    {
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
            return this.Failure();

        ActionInfo = new()
        {
            Actions = ActionInfo.Actions.Select(a =>
                new ActionData(a.Name, IsUserAction: true, a.DoneBefore, a.AlreadyMadeActionByPlayers, a.Repeat)).ToArray(),

            ExpectedPlayers = userIds,
            MustObeyOrder = mustObeyOrder,
        };

        return this.Success();
    }

    #endregion
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
    bool CanDoAction(Delegate @delegate, UserId? user = null) => CanMakeAction(@delegate.Method.Name, user);

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
            return this;

        return _controller.SetActionDone(name, userId);
    }

    public IChainedOperation SetActionExpectedNext(string name, ActionRepeat repeat = default)
    {
        if (!IsSuccess)
            return this;

        return _controller.SetActionExpectedNext(name, repeat);
    }

    public bool CanMakeAction(string name, UserId? user = null) =>
        _controller.CanMakeAction(name, user);

    public IChainedOperation By(UserId[] userIds, bool mustObeyOrder = false)
    {
        if (!IsSuccess)
            return this;

        return _controller.By(userIds, mustObeyOrder);
    }

    public IChainedOperation Or(string name, ActionRepeat repeat = ActionRepeat.Single)
    {
        if (!IsSuccess)
            return this;

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

public class Action
{
    public string[] Types { get; init; }
    public UserId[] ExpectedPlayers { get; init; } = Array.Empty<UserId>();
    public bool MustObeyOrder { get; init; }
    public List<UserId> AlreadyMadeActionsPlayers { get; } = new();

    public bool ExpectsUserAction() => !ExpectedPlayers.IsNullOrEmpty();
    public bool HasUsersDoneActions()
    {
        if (ExpectedPlayers.IsNullOrEmpty())
            return true;

        return ExpectedPlayers.HasSameElements(AlreadyMadeActionsPlayers);
    }

    public bool CanMakeAction(UserId? userId = null, string name = "") 
    {
        if (!name.IsNullOrEmpty() && !Types.Contains(name))
            return false;

        if (userId is not null && ExpectedPlayers.IsNullOrEmpty())
            return false;

        if (MustObeyOrder)
        {
            int expectedOrderIndex = Array.FindIndex(ExpectedPlayers, id => id == userId);
            if (AlreadyMadeActionsPlayers.Count == expectedOrderIndex)
                return true;
        }
        else
        {
            if (!AlreadyMadeActionsPlayers.Contains(userId))
                return true;
        }

        return false;
    }
}

public class ActionInfo
{
    public ActionData[] Actions { get; init; } = Array.Empty<ActionData>();
    public bool MustObeyOrder { get; init; }
    public UserId[] ExpectedPlayers { get; init; } = Array.Empty<UserId>();

    public bool CanMakeAction(UserId userId, string name = "")
    {
        // Check if given action is even expected 
        var action = Actions.FirstOrDefault(a => a.Name == name);
        if (!name.IsNullOrEmpty() && action is null)
            return false;

        // Check if user done any single time action before, which forbids to make any more
        var singleTimeActions = Actions.Where(a => a.Repeat is ActionRepeat.Single).ToArray();
        if (singleTimeActions.Any(a => a.AlreadyMadeActionByPlayers.Contains(userId)))
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

    public bool CanMakeAction(string name, UserId? user = null)
    {
        if (name.IsNullOrEmpty())
            return false;

        if (user is not null && ExpectedPlayers.IsNullOrEmpty())
            return false;

        // Check if user done any single time action before, which forbids to make any more
        var singleTimeActions = Actions.Where(a => a.Repeat is ActionRepeat.Single).ToArray();
        if (singleTimeActions.Any(a => !a.IsUserAction && a.DoneBefore))
            return false;

        // Check if given action is even expected 
        var action = Actions.FirstOrDefault(a => a.Name == name);
        if (!name.IsNullOrEmpty() && action is null)
            return false;

        return true;
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
            if (!singleTimeActions.Any(a => a.AlreadyMadeActionByPlayers.HasSameElements(ExpectedPlayers)))
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
        Actions.First(a => a.Name == name);
}

public record ActionData(
    string Name,
    bool IsUserAction,
    bool DoneBefore,
    UserId[] AlreadyMadeActionByPlayers,
    ActionRepeat Repeat = default);
