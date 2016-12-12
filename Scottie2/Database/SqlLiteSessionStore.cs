using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Dapper;

namespace Scottie.Database
{
    public class SqlLiteSessionStore : ISessionStore
    {
        private readonly string _dbFile;

        public SqlLiteSessionStore(string dbFile)
        {
            if (dbFile == null) throw new ArgumentNullException(nameof(dbFile));

            _dbFile = dbFile;
        }
        
        public SqliteConnection SimpleDbConnection()
        {
            var connection = new SqliteConnectionStringBuilder { DataSource = _dbFile };

            return new SqliteConnection(connection.ToString());
        }

        // not sure about the thread safety of this yet...
        public static object LockObject = new object();

        public void Init()
        {
            string path = Path.Combine(AppContext.BaseDirectory, _dbFile);
            if (File.Exists(path)) return;

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table Session
              (
                 ID               integer primary key AUTOINCREMENT,
                 LastHeartbeat     datetime not null,
              )");
            }
        }

        public long Create()
        {
            lock (LockObject)
            {
                using (SqliteConnection cnn = SimpleDbConnection())
                {
                    DateTime lastHeartbeat = DateTime.UtcNow;

                    long id = cnn.Query<long>(
                        @"INSERT INTO Session (LastHeartbeat) VALUES 
            (  @lastHeartbeat );
            select last_insert_rowid()", new { lastHeartbeat }).FirstOrDefault();

                    return id;
                }
            }
        }

        public void Heartbeat(long id)
        {
            using (var cnn = SimpleDbConnection())
            {
                DateTime heartbeat = DateTime.UtcNow;

                cnn.Execute(
                    @"UPDATE Session SET LastHeartbeat=@heartbeat WHERE ID = @id",
                    new {heartbeat, id});
            }
        }

        public void Delete(long id)
        {
            lock (LockObject)
            {
                using (var cnn = SimpleDbConnection())
                {
                    cnn.Execute(@"DELETE FROM Session WHERE ID = @id", new {id});
                }
            }
        }

        public bool Exists(long id)
        {
            lock (LockObject)
            {
                using (SqliteConnection cnn = SimpleDbConnection())
                {
                    long exists = cnn.Query<long>(
                        @"SELECT ID FROM Session WHERE ID = @id", 
                        new { id }).FirstOrDefault();
                    return id == exists;
                }
            }
        }

        public IImmutableList<Session> GetAllSessions()
        {
            lock (LockObject)
            {
                using (SqliteConnection cnn = SimpleDbConnection())
                {
                    return cnn.Query<Session>(@"SELECT ID, LastHeartbeat FROM Session")
                        .ToImmutableList();
                }
            }
        }
    }
}