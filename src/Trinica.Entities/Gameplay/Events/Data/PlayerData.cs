using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class PlayerData
{
    public PlayerData(UserId id, CardData[] handDeck, CardData[] battlingDeck)
    {
        Id = id;
        HandDeck = handDeck;
        BattlingDeck = battlingDeck;
    }

    public UserId Id { get; }
    public CardData[] HandDeck { get; }
    public CardData[] BattlingDeck { get; }
}
