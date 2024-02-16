using Corelibs.Basic.Events;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trinica.Api.Extensions;
using Trinica.ApiContracts.Games;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;
using Trinica.UseCases.Gameplay;
using Trinica.UseCases.Gameplay.Events;

namespace Trinica.Api.Controllers;

[Route("api/v1/games")]
[ApiController]
[Authorize]
public class GamesController(
    IMediator mediator,
    IEventsDispatcher<GameId, UserId, GameEvent> gameEventsDispatcher) : BaseController
{
    private readonly IMediator _mediator = mediator;
    private readonly IEventsDispatcher<GameId, UserId, GameEvent> _gameEventsDispatcher = gameEventsDispatcher;

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

    [HttpGet("{gameId}/events")]
    public async Task GetEvents(GetGameEventsApiQuery query)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");

        bool done = false;
        _gameEventsDispatcher.Subscribe(query.GameId, query.Body.PlayerId, onEvent: async ev =>
        {
            if (ev is GameFinishedOutEvent finishedEv)
            {
                done = true;
                return;
            }

            var dataJson = JsonConvert.SerializeObject(ev);
            await Response.WriteAsync($"data: {dataJson}\n\n");
            await Response.Body.FlushAsync();
        });

        while (!done && !HttpContext.RequestAborted.IsCancellationRequested)
            await Task.Delay(10);

        _gameEventsDispatcher.Unsubscribe(query.Body.PlayerId);
    }
}

public interface IData;
public record TestData(string Id, string Name) : IData;