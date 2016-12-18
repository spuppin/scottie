using System;

namespace Scottie.Server
{
    public class CreateParams
    {
        public string CreateMode { get; set; }
        public string Data { get; set; }

        public CreateMode GetMode()
        {
            return (CreateMode) Enum.Parse(typeof(CreateMode), CreateMode, true);
        }
    }
}