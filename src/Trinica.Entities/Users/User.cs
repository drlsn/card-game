using Corelibs.Basic.DDD;
using Newtonsoft.Json;
using Trinica.Entities.Gameplay;

namespace Trinica.Entities.Users;

public record UserId(string Value) : EntityId(Value);

[Serializable]
public class User : Entity<UserId>, IAggregateRoot<UserId>
{
    public const string DefaultCollectionName = "users";
    public User(UserId id) : base(id) {}
    public User(UserId id, uint version) : base(id, version) { }

    public string LastGameId { get; private set; }

    public void ChangeLastGame(GameId gameId) => LastGameId = gameId;
}
