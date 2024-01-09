using Corelibs.Basic.DDD;
using Trinica.Entities.Gameplay;

namespace Trinica.Entities.Users;

public class UserId : EntityId
{
    public UserId(string value) : base(value)
    {
    }
}

[Serializable]
public class User : Entity<UserId>, IAggregateRoot<UserId>
{
    public static string DefaultCollectionName { get; } = "users";

    public User(UserId id) : base(id) {}
    public User(UserId id, uint version) : base(id, version) { }

    public GameId LastGameId { get; private set; }

    public void ChangeLastGame(GameId gameId) => LastGameId = gameId;
}
