using System;

namespace Scottie.Session
{
    public class SessionResult
    {
        public SessionResult(string description, long sessionId)
            : this()
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));

            Description = description;
            SessionId = sessionId;
        }

        public SessionResult()
        {
        }

        public string Description { get; set; }
        public long SessionId { get; set; }
    }
}