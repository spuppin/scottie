using System;
using Microsoft.AspNetCore.Mvc;
using Scottie.Database;

namespace Scottie.Server
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly ISessionStore _store;

        public SessionController(ISessionStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            _store = store;
        }

        [HttpPost]
        public IActionResult Create()
        {
            long id = _store.Create();

            return new ObjectResult(new SessionResult ("Created!", id));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var sessions = _store.GetAllSessions();

            return new ObjectResult(sessions);
        }

        [HttpPut("{sessionId}")]
        public IActionResult Hearbeat(long sessionId)
        {
            _store.Heartbeat(sessionId);

            return new ObjectResult(new { status = "Heartbeat!", sessionId });
        }

        [HttpDelete("{sessionId}")]
        public IActionResult Delete(long sessionId)
        {
            _store.Delete(sessionId);

            return new ObjectResult(new SessionResult("Deleted!", sessionId ));
        }
    }
}