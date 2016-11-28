using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Scottie.Models;

namespace Scottie.Controllers
{
    [Route("znode")]
    public class ZNodeController : Controller
    {
        [HttpPost("{*path}")]
        public IActionResult Create(string path, [FromBody] CreateParams createParams)
        { 
            return new ObjectResult(new {status="Created!", path, createParams});
        }

        [HttpPut("{*path}")]
        public IActionResult Update(string path, [FromBody] UpdateParams updateParams)
        {
            return new ObjectResult(new { status = "Updated!", path, updateParams });
        }

        [HttpDelete("{*path}")]
        public IActionResult Delete(string path, [FromBody] DeleteParams deleteParams)
        {
            return new ObjectResult(new { status = "Deleted!", path, deleteParams });
        }

        [HttpPost("multi")]
        public IActionResult Multi([FromBody] IEnumerable<MultiOpParams> multiParams)
        {
            return new ObjectResult(new { status = "Multied!", multiParams });
        }

        [HttpGet("{*path}")]
        public IActionResult Get(string path)
        {
            return new ObjectResult(new { status = "Getted!", path });
        }

        [HttpGet("children/{*path}")]
        public IActionResult GetChildren(string path)
        {
            return new ObjectResult(new { status = "Children getted!", path });
        }
    }
}