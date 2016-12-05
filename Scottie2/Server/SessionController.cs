using System;
using Microsoft.AspNetCore.Mvc;
using Scottie.Session;

namespace Scottie.Server
{
    [Route("session")]
    public class SessionController : Controller
    {
        [HttpPost()]
        public IActionResult Create()
        {
            var random = new Random();
            return new ObjectResult(new SessionResult ("Created!",  (long)random.Next()));
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