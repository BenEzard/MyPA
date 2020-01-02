namespace MyPA.Code.Data.Services
{
    public class BaseRepository
    {
        /// <summary>
        /// The database file where the data us stored.
        /// </summary>
        public const string DatabaseFile = @"D:\MyPA.db";

        /// <summary>
        /// Connection string for the sqlite database.
        /// </summary>
        public static string dbConnectionString = "data source=" + DatabaseFile;
    }
}
