using Corelibs.Basic.Events;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trinica.Api.Extensions;
using Trinica.ApiContracts.Games;
using Trinica.UseCases.Gameplay;
using Trinica.UseCases.Gameplay.Events;

namespace Trinica.Api.Controllers;

[Route("api/v1/games")]
[ApiController]
[Authorize]
public class GamesController(
    IMediator mediator,
    IRoomEventsSubscriber gameEventsSubscriber) : BaseController
{
    private readonly IMediator _mediator = mediator;
    private readonly IRoomEventsSubscriber _gameEventsSubscriber = gameEventsSubscriber;

    //[HttpGet, Route("me")]
    //public Task<IActionResult> Get() =>
    //    _mediator.SendAndGetResponse(new GetUserQuery(UserID));

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        if (!User.Identity.IsAuthenticated)
            return BadRequest();

        var appCommand = new StartBotGameCommand(UserID);
        return await _mediator.SendAndGetPostResponse(appCommand);
    }

    [HttpPost("{gameId}/takeCardToHand")]
    public async Task<IActionResult> TakeCardToHand(TakeCardToHandApiCommand command)
    {
        if (!User.Identity.IsAuthenticated)
            return BadRequest();

        var appCommand = new TakeCardToHandCommand(command.GameId, command.Body.PlayerId, command.Body.CardToTakeId);
        return await _mediator.SendAndGetPostResponse(appCommand);
    }

    [HttpGet("{gameId}/events/subcribe")]
    public async Task GetEvents(GetGameEventsApiQuery query, CancellationToken ct)
    {
        var doneTcs = new TaskCompletionSource();

        Response.Headers.Append("Content-Type", "text/event-stream");

        if (!await _gameEventsSubscriber.Subscribe(
            query.LastEventIndex, query.GameId, UserID, 
            onEvent: async ev =>
            {
                await Response.WriteAsync("data: ");
                await Response.WriteAsync($"{ev.GetType().Name} ");
                await Response.WriteAsJsonAsync(ev);
                await Response.Body.FlushAsync();

                if (ev is GameFinishedOutEvent)
                    doneTcs.SetResult();
            }))
            return;

        ct.Register(() => _gameEventsSubscriber.Unsubscribe(UserID));
        await doneTcs.Task;
    }
}

public interface IData;
public record TestData(string Id, string Name) : IData;