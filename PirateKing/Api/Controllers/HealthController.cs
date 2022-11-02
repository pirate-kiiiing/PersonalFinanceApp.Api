using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers
{
    [Route("health")]
    [ApiController]
    [AllowAnonymous]

    public class HealthController : ControllerBase
    {
        [HttpGet, Route("")]
        public String Alive() => "alive";

#if DEBUG
        [HttpGet, Route("test")]
        public String Test()
        {
            return "test";
        }
#endif
    }
}
