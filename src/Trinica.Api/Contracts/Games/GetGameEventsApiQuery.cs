
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Trinica.ApiContracts.Games;

public class GetGameEventsApiQuery
{
    [FromRoute]
    public string GameId { get; set; }

    [FromQuery]
    public int? LastEventIndex { get; set; }
}
