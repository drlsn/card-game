namespace Trinica.UseCases.Gameplay.Events;

public record GameStartedOutEvent(string GameId);
public record GameFinishedOutEvent(string GameId);

public record CardsTakenToHandOutEvent(string GameId, string PlayerId);
