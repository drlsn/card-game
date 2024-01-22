using Corelibs.Basic.Collections;

namespace Corelibs.Basic.DDD;

public abstract class NamedEntity<TId> : Entity<TId>
    where TId : EntityId
{
    public string Name { get; private set; } = "Resource";

    public NamedEntity(string name)
    {
        Name = !name.IsNullOrEmpty() ? name : throw new InvalidDataException();
    }

    public bool Rename(string name)
    {
        if (name.IsNullOrEmpty())
            return false;

        Name = name;
        return true;
    }
}
