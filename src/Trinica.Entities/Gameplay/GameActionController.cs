using Corelibs.Basic.Collections;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public class GameActionController
{
    public Action ExpectedAction = new();

    public GameActionController(Delegate @delegate) => SetNextExpectedAction(@delegate.Method.Name);
    public GameActionController(Delegate @delegate, UserId[] expectedPlayers) => SetNextExpectedAction(@delegate.Method.Name, expectedPlayers);
    public GameActionController(string type) => SetNextExpectedAction(type);

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

    public bool CanMakeAction(UserId userId, string type = "") 
    {
        if (!type.IsNullOrEmpty() && !Types.Contains(type))
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
