using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trinica.Api.Extensions;
using Trinica.ApiContracts.Games;
using Trinica.UseCases.Gameplay;

namespace Trinica.Api.Controllers;

[Route("api/v1/games")]
[ApiController]
[Authorize]
public class GameController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

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
    public async Task GetEvents(string gameId)
    {
        var response = Response;
        response.Headers.Add("Content-Type", "text/event-stream");

        for (var i = 0; i < 10; i++)
        {
            await response.WriteAsync($"data: Bobolo {i + 1}\n\n");
            await response.Body.FlushAsync();
            await Task.Delay(1000);
        }
    }
}
