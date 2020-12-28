using Microsoft.AspNetCore.Mvc;

namespace AppV1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "this is the app version 1.0";
        }
    }
}
