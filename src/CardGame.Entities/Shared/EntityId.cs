namespace CardGame.Entities.Shared;

public abstract record Id<TId>(string Value)
{
    public static TId New() => (TId) Activator.CreateInstance(typeof(TId), Guid.NewGuid().ToString())!;
    public bool IsValid() => Guid.TryParse(Value, out var id);
}
