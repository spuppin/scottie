using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using Scottie.Server;
using Dapper;

namespace Scottie.Database
{
    public class SqlLiteNodeStore : SqlLiteStore, INodeStore
    {
        public SqlLiteNodeStore(string dbFile)
            : base(dbFile)
        {
        }

        public void Init()
        {
            if (DbExists()) return;

            using (IDbConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"CREATE TABLE Znode
              (
                 ID                   INTEGER PRIMARY KEY AUTOINCREMENT,
                 Parent               TEXT NOT NULL,
                 Child                TEXT,
                 EphemeralSessionId   INTEGER,
                 Data                 TEXT,
                 Version              INTEGER
              )");
            }
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
