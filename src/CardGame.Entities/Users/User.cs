using CardGame.Entities.Shared;

namespace CardGame.Entities.Users;

public record UserId(string Value) : Id<UserId>(Value);

public class User : Entity<UserId>
{
    public User(UserId id) : base(id) {}
    public User(UserId id, uint version) : base(id, version) {}
}
