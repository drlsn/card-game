
using Microsoft.AspNetCore.Mvc;

namespace Trinica.ApiContracts.Games;

public class TakeCardToHandApiCommand
{
    [FromRoute]
    public string GameId { get; set; }

    [FromBody]
    public BodyDto Body { get; set; }

    public class BodyDto
    {
        public string PlayerId { get; set; }
        public string CardToTakeId { get; set; }
    }
}
