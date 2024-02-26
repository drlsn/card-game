
using Microsoft.AspNetCore.Mvc;

namespace Trinica.ApiContracts.Games;

public class StartGameApiCommand
{
    [FromBody]
    public BodyDto Body { get; set; }

    public class BodyDto
    {
        public bool Random { get; set; }
        public bool Bot { get; set; }
    }
}
