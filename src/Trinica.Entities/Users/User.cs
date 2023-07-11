using Corelibs.Basic.DDD;

namespace Trinica.Entities.Users;

public record UserId(string Value) : EntityId(Value);

public class User : Entity<UserId>, IAggregateRoot<UserId>
{
    public const string DefaultCollectionName = "users";
    public User(UserId id) : base(id) {}
    public User(UserId id, uint version) : base(id, version) {}
}
