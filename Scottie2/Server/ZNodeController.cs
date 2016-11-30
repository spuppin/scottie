using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Scottie.Server
{
    [Route("znode")]
    public class ZNodeController : Controller
    {
        [HttpPost("{sessionId}/{*path}")]
        public IActionResult Create(long sessionId, string path, [FromBody] CreateParams createParams)
        { 
            return new ObjectResult(new {status="Created!", sessionId, path, createParams});
        }

        [HttpPut("{sessionId}/{*path}")]
        public IActionResult Update(long sessionId, string path, [FromBody] UpdateParams updateParams)
        {
            return new ObjectResult(new { status = "Updated!", sessionId, path, updateParams });
        }

        [HttpDelete("{sessionId}/{*path}")]
        public IActionResult Delete(long sessionId, string path, [FromBody] DeleteParams deleteParams)
        {
            return new ObjectResult(new { status = "Deleted!", sessionId, path, deleteParams });
        }

        [HttpPost("{sessionId}/multi")]
        public IActionResult Multi(long sessionId, [FromBody] IEnumerable<MultiOpParams> multiParams)
        {
            return new ObjectResult(new { status = "Multied!", sessionId, multiParams });
        }

        [HttpGet("{sessionId}/{*path}")]
        public IActionResult Get(long sessionId, string path)
        {
            return new ObjectResult(new { status = "Getted!", sessionId, path });
        }

        [HttpGet("{sessionId}/children/{*path}")]
        public IActionResult GetChildren(long sessionId, string path)
        {
            return new ObjectResult(new { status = "Children getted!", sessionId, path });
        }
    }
}