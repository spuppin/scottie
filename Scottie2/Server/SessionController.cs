using Microsoft.AspNetCore.Mvc;

namespace Scottie.Server
{
    [Route("session")]
    public class SessionController : Controller
    {
        [HttpPost("{sessionId}")]
        public IActionResult Create(long sessionId)
        { 
            return new ObjectResult(new {status="Created!", sessionId});
        }

        [HttpPut("{sessionId}")]
        public IActionResult Hearbeat(long sessionId)
        {
            return new ObjectResult(new { status = "Hearbeat!", sessionId });
        }

        [HttpDelete("{sessionId}")]
        public IActionResult Delete(long sessionId)
        {
            return new ObjectResult(new { status = "Deleted!", sessionId });
        }
    }
}