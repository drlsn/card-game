
using Microsoft.AspNetCore.Mvc;

namespace Trinica.ApiContracts.Games;

public class GetGameStateApiQuery
{
    [FromRoute]
    public string GameId { get; set; }
}
