using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Scottie.Models;

namespace Scottie.Controllers
{
    [Route("api/[controller]")]
    public class ZNodeController : Controller
    {
          Good progress. This worked when I had a basic /get HttpGet action
          NEXT: implement stub versions of the methods below.

        public ZNodeController()
        {
            
        }
        [HttpPost("{*path}")]
        public IActionResult Create(string path, [FromBody] CreateParams createParams)
        {
            throw new NotImplementedException();
            //return Created("znode", "foo");
        }

        [HttpPut("{*path}")]
        public IActionResult Update(string path, [FromBody] UpdateParams updateParams)
        {
            throw new NotImplementedException();
            //return Created("znode", "foo");
        }

        [HttpDelete("{*path}")]
        public IAsyncResult Delete(string path, [FromBody] DeleteParams deleteParams)
        {
            throw new NotImplementedException();
        }

        [HttpPost("multi")]
        public IAsyncResult Multi([FromBody] List<MultiOpParams> multiParams)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{*path}")]
        public IAsyncResult Get(string path)
        {
            throw new NotImplementedException();
        }

        [HttpGet("children/{*path}")]
        public IAsyncResult GetChildren(string path)
        {
            throw new NotImplementedException();
        }
    }
}