namespace CardGame.Entities.Shared
{
    public interface IEntity<TId>
    {
        TId Id { get; }
        uint Version { get; set; }
    }

    public class Entity<TId> : IEntity<TId>
    {
        public TId Id { get; }

        private uint _version;

        public Entity(TId id)
        {
            Id = id;
        }

        public Entity(TId id, uint version)
        {
            Id = id;
            _version = version;
        }

        uint IEntity<TId>.Version { get => _version; set { _version = value; } }

        public static implicit operator bool(Entity<TId> entity) => entity != null;
    }
}
