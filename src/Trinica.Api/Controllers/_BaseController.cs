using Microsoft.AspNetCore.Mvc;
using Trinica.Api.Authorization;

namespace Trinica.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public string UserID => User.GetUserID();
    }
}
