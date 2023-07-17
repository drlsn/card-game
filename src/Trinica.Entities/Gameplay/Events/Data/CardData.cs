using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Events;

public class CardData
{
    public CardData(CardId id, string type)
    {
        Id = id;
        Type = type;
    }

    public CardId Id { get; }
    public string Type { get; }
}
