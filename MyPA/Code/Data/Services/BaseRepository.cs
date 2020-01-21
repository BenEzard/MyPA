using System;
using System.Collections.Generic;
using System.Data.SQLite;

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

        /// <summary>
        /// Get preferences for the specified viewModel. If no viewModel is provided, all preferences are returned.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        internal Dictionary<PreferenceName, Preference> GetPreferences(string viewModel)
        {
            var rValue = new Dictionary<PreferenceName, Preference>();

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Preference";
                    if (viewModel == null)
                    {
                        cmd.CommandText = sql;
                    }
                    else
                    {
                        sql += " WHERE (Model = @model)";
                        cmd.Parameters.AddWithValue("@model", viewModel);
                        cmd.CommandText = sql;
                    }

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
