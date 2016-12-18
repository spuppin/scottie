using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using Scottie.Server;
using Dapper;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Scottie.Database
{
    public class SqlLiteNodeStore : SqlLiteStore, INodeStore
    {
        private long currentVersion;
        public SqlLiteNodeStore(string dbFile)
            : base(dbFile)
        {
        }

        public bool TableExists()
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                long id = cnn.Query<long>(
                    "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = 'Znode';")
                    .FirstOrDefault();
                if (id == 1)
                    return true;
            }
            return false;
        }

        public void Init()
        {
            if (TableExists())
            {
                // TODO: get current version
                return;
            }

            using (IDbConnection cnn = SimpleDbConnection())
            {
                //cnn.Open();
                cnn.Execute(
                    @"CREATE TABLE Znode
              (
                 ID                   INTEGER PRIMARY KEY AUTOINCREMENT,
                 ParentPath               TEXT NOT NULL,
                 Path                TEXT,
                 EphemeralSessionId   INTEGER,
                 Data                 TEXT,
                 Version              INTEGER,
                 SequenceNum          INTEGER
              )");
            }

            currentVersion = 1;
        }

        private long GetMaxVersion()
        {
            // TODO: SELECT MAX(VERSION) FROM ZNode
            throw new NotImplementedException();
        }


        public string Create(long sessionId, string path, CreateMode createMode, string data)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            using (var cnn = SimpleDbConnection())
            {
                string parent;
                
                long? ephemeralSessionId = null;
                int index = path.LastIndexOf('/');
                if (index == -1)
                {
                    parent = null;
                    path = "/" + path;
                }
                else if (index == 0)
                {
                    parent = null;
                }
                else
                {
                    parent = path.Substring(0, index);
                    path = path.Substring(index, path.Length - index);
                }

                if (createMode == CreateMode.Ephemeral || createMode == CreateMode.EphemeralSequential)
                {
                    ephemeralSessionId = sessionId;
                }                

                using (IDbTransaction tr = cnn.BeginTransaction())
                {
                    path = GetCreationPath(cnn, tr, parent, path, createMode);

                    long version = ++currentVersion;
                    long sequenceNum = 0;
                    // Have different ephemeral session query..?
                    cnn.Execute(
                            @"INSERT INTO Znode (ParentPath, Path, EphemeralSessionId, Data, Version, SequenceNum) VALUES 
            (  @parent, @child, @ephemeralSessionId, @data, @version, @sequenceNum );",
                            new { parent, path, ephemeralSessionId, data, version, sequenceNum }, tr);

                    tr.Commit();
                }
               

            return path;
            }
        }

        string GetCreationPath(IDbConnection cnn, IDbTransaction tr, string parent, string path, CreateMode mode)
        {
            if (mode == CreateMode.Ephemeral || mode == CreateMode.Persistent)
                return path;

            long version = ++currentVersion;
            cnn.Execute(
                        @"UPDATE ZNode SET Version = @version, SequenceNum = SequenceNum + 1 WHERE PATH = @parent",
                        new { version, parent }, tr);

            long sequenceNum = cnn.Query<long>("SELECT SequenceNum FROM ZNode where ParentPath = @parentPath",
                new { parent }).FirstOrDefault();

            // Aha.. so now make path better
            path = path + sequenceNum.ToString("D10");
            return path;
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
