namespace CardGame.Entities.Shared
{
    public abstract class Entity<TId> : IEntity<TId>
        where TId : Id<TId>
    {
        public TId Id { get; }

        public uint Version { get; private set; }

        public Entity() => Id = Id<TId>.New();
        public Entity(TId id) => Id = id;

        public Entity(TId id, uint version)
        {
            Id = id;
            Version = version;
        }

        uint IEntity<TId>.Version { get => Version; set { Version = value; } }

        public static implicit operator bool(Entity<TId> entity) => entity != null;
    }
}
