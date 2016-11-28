using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scottie
{public enum CreateMode { }
    public class Op { }

    public class Watcher { }

    public interface IScottie
    {
        // all of these need a sessionid.

        /*

        { 
          "sessionid": 1234,
          "path": "/foo/bar",
          "createmode" : "persistent",
          "data": ""
          "version" : 5678
            http://stackoverflow.com/questions/472906/how-to-get-a-consistent-byte-representation-of-strings-in-c-sharp-without-manual
         }

         */

        // How-to call the API.
        // https://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client

        // POST localhost:8080/session
        long CreateSession();
        // DELETE localhost:8080/session
        void DeleteSession();
        // PUT localhost:8080/session
        void SessionHeartbeat();


        // I think I can have URLs like this:
        // POST http://localhost:8080/znode/persistent/foo/bar
        // [Route("znode/{createmode}/{*path}")]

        // GET localhost:80/znode/children
        List<string> GetChildren();
        /*
         * 
         * 
         * void Multi(List<Op> ops);
* create(final String path, byte data[], CreateMode createMode)
* delete(final String path, int version)
* multi(Op op)
** class Create : Op { ... }
** class Delete : Op { ... }
** class SetData : Op { ... }
** class Check : Op { ... }
* public List<String> getChildren(final String path, Watcher watcher)
* sync.Flushes channel between process and leader.
}*/
    }

    public class Scottie
    {
    }
}
