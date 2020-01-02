using MyPA.Data;
using MyPA.Code;
using MyPA.Code.Data.Services;

namespace DBBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbConnectionString = "data source=" + @"D:\MyPA.db";
            new DBInstaller(BaseRepository.DatabaseFile, dbConnectionString);
        }
    }
}
