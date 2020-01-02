using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MyPA.Code.Data.Services
{
    public class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        Dictionary<PreferenceName, Preference> IApplicationRepository.GetApplicationPreferences()
        {
            var rValue = new Dictionary<PreferenceName, Preference>();

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "SELECT * FROM Setting";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string settingNameStr = (string)reader["Name"];
                            Enum.TryParse(settingNameStr, out PreferenceName preferenceName);
                            string preferenceValue = (string)reader["Value"];
                            string defaultValue = (string)reader["DefaultValue"];
                            string description = (string)reader["Description"];
                            string userCanEditChar = (string)reader["UserCanEdit"];
                            bool userCanEdit = (userCanEditChar.Equals("Y")) ? true : false;

                            rValue.Add(preferenceName, new Preference(preferenceName, preferenceValue, defaultValue, description, userCanEdit));
                        }
                    }
                    connection.Close();
                }
            }

            return rValue;
        }
    }
}
