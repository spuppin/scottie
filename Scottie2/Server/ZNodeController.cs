using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Scottie.ZNode;

namespace Scottie.Server
{
    [Route("znode")]
    public class ZNodeController : Controller
    {
        [HttpPost("{sessionId}/{*path}")]
        public IActionResult Create(long sessionId, string path, [FromBody] CreateParams createParams)
        {
            return new ObjectResult(new CreateResult {Path = path});
        }

        [HttpPut("{sessionId}/{*path}")]
        public IActionResult Update(long sessionId, string path, [FromBody] UpdateParams updateParams)
        {
            return new ObjectResult(new UpdateResult { Version = updateParams.Version} );
        }

        [HttpDelete("{sessionId}/{version}/{*path}")]
        public IActionResult Delete(long sessionId, string path, long version)
        {
            return new ObjectResult(new DeleteResult {Version = version});
        }

        [HttpPost("{sessionId}/multi")]
        public IActionResult Multi(long sessionId, [FromBody] IEnumerable<MultiOpParams> multiParams)
        {
            return new ObjectResult(new { status = "Multied!", sessionId, multiParams });
        }

        [HttpGet("{sessionId}/{*path}")]
        public IActionResult Get(long sessionId, string path)
        {
            return new ObjectResult(new GetResult {Path = path});
        }

        [HttpGet("{sessionId}/children/{*path}")]
        public IActionResult GetChildren(long sessionId, string path)
        {
            return new ObjectResult(new ChildrenResult { Children = new List<string> {path} });
        }
    }
}