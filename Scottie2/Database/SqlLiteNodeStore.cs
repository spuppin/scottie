using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using Scottie.Server;
using Dapper;
using System.Linq;
using System.Runtime.Serialization;

namespace Scottie.Database
{
    public class SqlLiteNodeStore : SqlLiteStore, INodeStore
    {
        private long _currentVersion;
        public SqlLiteNodeStore(string dbFile)
            : base(dbFile)
        {
            _currentVersion = 1;
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
                _currentVersion = GetMaxVersion();
                return;
            }

            using (IDbConnection cnn = SimpleDbConnection())
            {
                cnn.Execute(
                    @"CREATE TABLE Znode
              (
                 ID                   INTEGER PRIMARY KEY AUTOINCREMENT,
                 ParentPath               TEXT NOT NULL,
                 Path                TEXT,
                 EphemeralSessionId   INTEGER,
                 Datas                 TEXT,
                 Version              INTEGER,
                 SequenceNum          INTEGER
              )");
            }

            _currentVersion = InitialVersion;
        }

        private const long InitialVersion = 1;

        private long GetMaxVersion()
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                long version = cnn
                    .Query<long>("SELECT MAX(Version) as version From ZNode")
                    .FirstOrDefault();

                return version == 0 ? InitialVersion : version;
            }
        }

        public class Paths
        {
            public string ParentPath { get; }
            public string Path { get; }

            public Paths(string parentPath, string path)
            {
                if(string.IsNullOrWhiteSpace(path))
                    throw new ArgumentNullException(nameof(path));

                ParentPath = parentPath;
                Path = path;
            }
        }

        public Paths GetPaths(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string parent;
            int index = path.LastIndexOf('/');
            if (index == -1)
            {
                parent = null;
                path = "/" + path;
            }
            else if (index == 0)
            {
                parent = null;
                path = "/" + path;
            }
            else
            {
                parent = path.Substring(0, index);
                path = path.Substring(index, path.Length - index);
            }

            return new Paths(parent, path);
        }

        private string GetSequentialPath(IDbConnection cnn, IDbTransaction tr, string parent, string path, CreateMode mode)
        {
            if (mode != CreateMode.EphemeralSequential && 
                mode != CreateMode.PersitentSequential)
                return path;

            long version = ++_currentVersion;
            cnn.Execute(
                        @"UPDATE ZNode SET Version = @version, SequenceNum = SequenceNum + 1 WHERE PATH = @parent",
                        new { version, parent }, tr);

            long sequenceNum = cnn.Query<long>("SELECT SequenceNum FROM ZNode where ParentPath = @parentPath",
                new { parent }).FirstOrDefault();

            path = path + sequenceNum.ToString("D10");
            return path;
        }

        public static string GetSequentialPath(string path, long sequenceNum)
        {
            return path + sequenceNum.ToString("D10");
        }

        private static long? GetEphemeralSessionId(CreateMode mode, long sessionId)
        {
            if (mode == CreateMode.Ephemeral || mode == CreateMode.EphemeralSequential)
            {
                return  sessionId;
            }

            return null;
        }
        
        private string CreateInternal(long sessionId, string path, CreateMode createMode, string data, IDbConnection cnn,
            IDbTransaction tr)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            long? ephemeralSessionId = GetEphemeralSessionId(createMode, sessionId);

            Paths paths = GetPaths(path);
            string parentPath = paths.ParentPath;
            path = paths.Path;

            path = GetSequentialPath(cnn, tr, parentPath, path, createMode);

            long version = ++_currentVersion;
            cnn.Execute(
                @"INSERT INTO Znode (ParentPath, Path, EphemeralSessionId, Datas, Version, SequenceNum) 
                          VALUES(  @parentPath, @path, @ephemeralSessionId, @data, @version, 1 );",
                new {parentPath, path, ephemeralSessionId, data, version}, tr);

            tr.Commit();

            return path;
        }

        private long UpdateInternal(IDbConnection cnn, IDbTransaction tr, string path, string data, long version)
        {
            long newVersion = _currentVersion++;
            int rowsUpdated = cnn.Execute(
                @"UPDATe Znode SET Datas=@data, Version=@newVersion WHERE Path=@path AND Version = @version",
                new {data, newVersion, path, version});

            if (rowsUpdated == 0)
                throw new InvalidOperationException($"Failed to update {path} with version {version}");

            tr.Commit();

            return newVersion;
        }

        private static bool DeleteInternal(string path, long version, IDbConnection cnn)
        {
            int rowsUpdated = cnn.Execute(
                @"DELETE FROM Znode WHERE Path=@path AND Version = @version",
                new {path, version});

            if (rowsUpdated == 0)
                throw new InvalidOperationException($"Failed to delete {path} with version {version}");

            return true;
        }

        public string Create(long sessionId, string path, CreateMode createMode, string data)
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                using (IDbTransaction tr = cnn.BeginTransaction())
                {
                    return CreateInternal(sessionId, path, createMode, data, cnn, tr);
                }
            }
        }

        public long Update(string path, string data, long version)
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                using (IDbTransaction tr = cnn.BeginTransaction())
                {
                    return UpdateInternal(cnn, tr, path, data, version);
                }
            }
        }

        public bool Delete(string path, long version)
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                return DeleteInternal(path, version, cnn);
            }
        }

        public void Multi(long sessionId, IEnumerable<MultiOpParams> operations)
        {
            using (IDbConnection cnn = SimpleDbConnection())
            {
                using (IDbTransaction tr = cnn.BeginTransaction())
                {
                    foreach (MultiOpParams op in operations)
                    {
                        if (op.CheckVersion != null)
                            throw new NotImplementedException();
                        if (op.Create != null)
                        {
                            CreateInternal(sessionId, 
                                op.Path, 
                                op.Create.GetMode(), 
                                op.Create.Data, 
                                cnn, 
                                tr);
                        }
                        else if (op.Update != null)
                        {
                            UpdateInternal(cnn, tr, op.Path, op.Update.Data, op.Update.Version);
                        }
                        else if (op.Delete != null)
                        {
                            DeleteInternal(op.Path, op.Delete.Version, cnn);
                        }
                        else
                        {
                            throw new InvalidDataException("Operation must be CheckVersion, Create, Update or Delete.");
                        }
                    }
                }
            }
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
