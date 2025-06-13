using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACCI_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionInformationController : ControllerBase
    {
        [HttpPost("free")]
        public ActionResult<string> FreeExtension()
        {
            // Logic for free extension
            return Ok("Free extension applied successfully.");
        }
        [HttpPost("paid")]
        public ActionResult<string> PaidExtension()
        {
            // Logic for paid extension
            return Ok("Paid extension applied successfully.");
        }
    }
}
