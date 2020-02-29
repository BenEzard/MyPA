using MyPA.Data;
using MyPA.Code;
using MyPA.Code.Data.Services;
using System;

namespace DBBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            bool loadData = false;

            Console.WriteLine("DBBuilder\n\n");
            Console.WriteLine("Do you want to load data? (Y=Yes)");

            var key = Console.ReadKey();


            if ((key.KeyChar == 'Y') || (key.KeyChar == 'y'))
            {
                loadData = true;
            }

            string dbConnectionString = "data source=" + @"D:\MyPA.db";
            new DBInstaller(BaseRepository.DatabaseFile, dbConnectionString, loadData);
        }
    }
}
