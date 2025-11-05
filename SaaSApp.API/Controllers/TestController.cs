using Microsoft.AspNetCore.Mvc;

namespace SaaSApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

            [HttpGet(Name = "test")]
        public IActionResult Get() => Ok("Backend is running!");
    }
}
