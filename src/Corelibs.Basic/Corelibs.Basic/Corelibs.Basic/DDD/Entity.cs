using Corelibs.Basic.Collections;
using Corelibs.Basic.Repository;
using Mediator;

namespace Corelibs.Basic.DDD
{
    public abstract class Entity<TId> : IEntity<TId>
        where TId : EntityId
    {
        public TId Id { get; init; }

        public uint Version { get; private set; }

        public Entity() => Id = EntityId.New<TId>();
        public Entity(TId id) => Id = id;

        [Ignore]
        public List<IDomainEvent> DomainEvents { get; private set; } = new();

        public Entity(TId id, uint version)
        {
            Id = id;
            Version = version;
        }

        uint IEntity.Version { get => Version; set { Version = value; } }

        public static implicit operator bool(Entity<TId> entity) => entity != null;

        protected void Add(IDomainEvent @event) => DomainEvents.Add(@event);

        public override bool Equals(object? obj)
        {
            if (obj is Entity<TId> entity) 
                return Id == entity.Id;
                
            return base.Equals(obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            var result = Id.ToString();

            var type = GetType();
            var nameProperty = type.GetProperty("Name");
            if (nameProperty != null)
            {
                var name = nameProperty.GetValue(this) as string;
                if (!name.IsNullOrEmpty())
                    result = $"{name} - {Id.ToString()}";
            }

            return result;
        }

        public static string GetRandomName() =>
            $"Item - {new string(Guid.NewGuid().ToString().Take(8).ToArray())}";
    }
}
