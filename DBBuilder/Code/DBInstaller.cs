using System;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace MyPA.Data
{
    /// <summary>
    /// This method can be used to initialise the database.
    /// </summary>
    class DBInstaller
    {
        /// <summary>
        /// The path where all of the SQL files that should be executed are.
        /// Their names should confer the run-order.
        /// </summary>
        public const string SQLFileLocation = @"D:\Development\Repos\MyPA\DBBuilder\SQL";
        private string DBFilePath { get; set; }
        private string DBConnectionString { get; set; }

        public DBInstaller(string dbFilePath, string dbConnectionString)
        {
            DBFilePath = dbFilePath;
            DBConnectionString = dbConnectionString;

            Console.WriteLine($"Overwriting the file {DBFilePath}");

            SQLiteConnection.CreateFile(DBFilePath);

            LoadTables();
            LoadViews();
            InsertRecords();
        }

        private void InsertRecords()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation+"\\Data", "*.recs");

            using (var connection = new SQLiteConnection(DBConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        Console.WriteLine($"Loading records: {filePath}");
                        string fileContents = File.ReadAllText(filePath);
                        string[] inserts = fileContents.Split(new string[] { "INSERT INTO " }, StringSplitOptions.None);
                        Console.WriteLine("Number of records: " + inserts.Length);
                        foreach (string token in inserts) {
                            if (token.Length > 0)
                            {
                                string sql = "INSERT INTO " + token;
                                cmd.CommandText = sql;
                                Console.WriteLine(sql);
                                Thread.Sleep(500);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                connection.Close();
            }
        }
    

        /// <summary>
        /// Load all of the SQL tables in the SQLFileLocation.
        /// </summary>
        private void LoadTables()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation+"\\Tables", "*.sql");

            using (var connection = new SQLiteConnection(DBConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        Console.WriteLine($"Loading SQL file: {filePath}");
                        cmd.CommandText = File.ReadAllText(filePath);
                        cmd.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
//        }
        }

        private void LoadViews()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation + "\\Views", "*.sql");

            using (var connection = new SQLiteConnection(DBConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        Console.WriteLine($"Loading SQL file: {filePath}");
                        cmd.CommandText = File.ReadAllText(filePath);
                        cmd.ExecuteNonQuery();
                    }

                }
                connection.Close();
            }
        }
    }
}
