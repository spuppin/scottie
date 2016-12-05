using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Linq;
using Dapper;

namespace Scottie.Database
{
    public class ScottiesHole
    {
        public static string DbFile => "scottie.db";

        public static SqliteConnection SimpleDbConnection()
        {
            var connection = new SqliteConnectionStringBuilder
            {
                DataSource = DbFile
            };

            return new SqliteConnection(connection.ToString());
        }

        // not sure about the thread safety of this yet...
        public static object lockObject = new object();

        public static void CreateDatabase()
        {
            string path = Path.Combine(AppContext.BaseDirectory, DbFile);
            if (File.Exists(path)) return;

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table Session
              (
                 ID               integer primary key AUTOINCREMENT,
                 CreationDate     datetime not null
              )");
            }
        }


        public static long CreateSession()
        {
            lock (lockObject)
            {
                using (var cnn = SimpleDbConnection())
                {
                    var creationDate = DateTime.UtcNow;

                    long id = cnn.Query<long>(
                        @"INSERT INTO Session (CreationDate) VALUES 
            (  @creationDate );
            select last_insert_rowid()", new {creationDate}).FirstOrDefault();

                    return id;
                }
            }
        }

        public static void DeleteSession(long id)
        {
            lock (lockObject)
            {
                using (var cnn = SimpleDbConnection())
                {
                    cnn.Execute(@"DELETE FROM Session WHERE ID = @id", new {id});
                }
            }
        }
    }
}