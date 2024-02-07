using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trinica.Api.Extensions;
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
}
