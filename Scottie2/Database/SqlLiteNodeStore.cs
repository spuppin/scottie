using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Scottie.Server;

namespace Scottie.Database
{
    public class SqlLiteNodeStore : INodeStore
    {
        private readonly string _dbName;

        public SqlLiteNodeStore(string dbName)
        {
            if (dbName == null) throw new ArgumentNullException(nameof(dbName));

            _dbName = dbName;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public string Create(string path, string createMode, string data)
        {
            throw new NotImplementedException();
        }

        public long Update(string path, string data, long version)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string path, long version)
        {
            throw new NotImplementedException();
        }

        public void Multi(IEnumerable<MultiOpParams> operations)
        {
            throw new NotImplementedException();
        }

        public ZNode Get(string path)
        {
            throw new NotImplementedException();
        }

        public IImmutableList<string> GetChildren(string path)
        {
            throw new NotImplementedException();
        }
    }
}
