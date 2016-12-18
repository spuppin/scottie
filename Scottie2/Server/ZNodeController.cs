using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Scottie.Database;
using Scottie.Results;

namespace Scottie.Server
{
    public class ZNode
    {
        public string Path { get; set; }
        public long Version { get; set; }
        public string Data { get; set; }
    }


    [Route("znode")]
    public class ZNodeController : Controller
    {
        private readonly ISessionStore _sessions;
        private readonly INodeStore _nodes;

        public ZNodeController(ISessionStore sessions, INodeStore nodes)
        {
            if (sessions == null) throw new ArgumentNullException(nameof(sessions));
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            _sessions = sessions;
            _nodes = nodes;
        }

        private static IActionResult InvalidSession()
        {
            return new ObjectResult(new { Error = "Session does not exist" });
        }

        [HttpPost("{sessionId}/{*path}")]
        public IActionResult Create(long sessionId, string path, [FromBody] CreateParams createParams)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            path = _nodes.Create(sessionId, path, createParams.GetMode(), createParams.Data);

            return new ObjectResult(new CreateResult {Path = path});
        }        

        [HttpPut("{sessionId}/{*path}")]
        public IActionResult Update(long sessionId, string path, [FromBody] UpdateParams updateParams)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            long version = _nodes.Update(path, updateParams.Data, updateParams.Version);

            return new ObjectResult(new UpdateResult { Version = version} );
        }

        [HttpDelete("{sessionId}/{version}/{*path}")]
        public IActionResult Delete(long sessionId, string path, long version)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            bool deleted = _nodes.Delete(path, version);

            return new ObjectResult(new DeleteResult {Deleted = deleted});
        }

        [HttpPost("{sessionId}/multi")]
        public IActionResult Multi(long sessionId, [FromBody] IEnumerable<MultiOpParams> multiParams)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            _nodes.Multi(multiParams);

            return new ObjectResult(new { status = "Multied!", sessionId });
        }

        [HttpGet("{sessionId}/{*path}")]
        public IActionResult Get(long sessionId, string path)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            ZNode node = _nodes.Get(path);

            return new ObjectResult(new GetResult { Node = node});
        }

        [HttpGet("{sessionId}/children/{*path}")]
        public IActionResult GetChildren(long sessionId, string path)
        {
            if (!_sessions.Exists(sessionId)) return InvalidSession();

            IImmutableList<string> children = _nodes.GetChildren(path);

            return new ObjectResult(new ChildrenResult { Children = children });
        }
    }
}