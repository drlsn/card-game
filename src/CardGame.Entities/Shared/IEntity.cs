namespace CardGame.Entities.Shared
{
    public interface IEntity<TId>
    {
        TId Id { get; }
        uint Version { get; set; }
    }
}
