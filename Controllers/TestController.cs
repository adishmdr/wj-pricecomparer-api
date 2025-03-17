
using Microsoft.AspNetCore.Mvc;

namespace wj_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Test controller is working");
        }
    }
}