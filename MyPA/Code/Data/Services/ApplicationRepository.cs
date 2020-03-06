using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MyPA.Code.Data.Services
{
    public sealed class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        private static readonly ApplicationRepository _instance = new ApplicationRepository();

        static ApplicationRepository() { }

        private ApplicationRepository() { }

        public static ApplicationRepository Instance
        {
            get => _instance;
        }

        Dictionary<PreferenceName, Preference> IApplicationRepository.GetApplicationPreferences()
        {
            return this.GetPreferences("Application");
        }

        /// <summary>
        /// Update Preference value in the database.
        /// </summary>
        /// <param name="preference"></param>
        /// <param name="value"></param>
        public void UpdatePreference(PreferenceName preference, string value)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "UPDATE Preference" +
                        " SET [Value] = @value" +
                        " WHERE [Name] = @name" +
                        " AND [UserCanEdit] = 'Y'";
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@name", preference.ToString());
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
