using Corelibs.Basic.DDD;
using Trinica.Entities.Gameplay;

namespace Trinica.Entities.Users;

[Serializable]
public class User : Entity<UserId>, IAggregateRoot<UserId>
{
    public static string DefaultCollectionName { get; } = "users";

    public User(UserId id) : base(id) {}
    public User(UserId id, uint version) : base(id, version) { }

    public int TutorialStep { get; private set; }
    public GameId LastGameId { get; private set; }

    public void ChangeLastGame(GameId gameId) => LastGameId = gameId;
}

public class UserId(string value) : EntityId(value);
