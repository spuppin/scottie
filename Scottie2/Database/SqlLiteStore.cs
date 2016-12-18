using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Scottie.Database
{
    public class SqlLiteStore
    {
        private readonly string _dbFile;

        public SqlLiteStore(string dbFile)
        {
            if (dbFile == null) throw new ArgumentNullException(nameof(dbFile));

            _dbFile = dbFile;
        }

        protected IDbConnection SimpleDbConnection()
        {
            var connection = new SqliteConnectionStringBuilder { DataSource = _dbFile };

            return new SqliteConnection(connection.ToString());
        }

        //public bool DbExists()
        //{
        //    string path = Path.Combine(AppContext.BaseDirectory, _dbFile);
        //    return File.Exists(path);
        //}
    }
}