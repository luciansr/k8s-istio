using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AppV1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            return new 
            { 
                message = $"This is an Echo message!",
                version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            };
        }
    }
}
