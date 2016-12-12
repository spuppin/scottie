using System;

namespace Scottie.Database
{
    public class Session
    {
        public long Id { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}