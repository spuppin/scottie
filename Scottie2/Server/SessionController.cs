using System;
using Microsoft.AspNetCore.Mvc;
using Scottie.Database;
using Scottie.Session;

namespace Scottie.Server
{
    [Route("session")]
    public class SessionController : Controller
    {
        [HttpPost]
        public IActionResult Create()
        {
            long id = ScottiesHole.CreateSession();

            return new ObjectResult(new SessionResult ("Created!", id));
        }

        [HttpPut("{sessionId}")]
        public IActionResult Hearbeat(long sessionId)
        {
            return new ObjectResult(new { status = "Hearbeat!", sessionId });
        }

        [HttpDelete("{sessionId}")]
        public IActionResult Delete(long sessionId)
        {
            ScottiesHole.DeleteSession(sessionId);

            return new ObjectResult(new SessionResult("Deleted!", sessionId ));
        }
    }
}