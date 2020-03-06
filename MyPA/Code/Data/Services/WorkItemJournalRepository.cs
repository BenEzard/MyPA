using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MyPA.Code.Data.Services
{
    public class WorkItemJournalRepository : BaseRepository, IWorkItemJournalRepository
    {

        /// <summary>
        /// Load all of the Journals for a specified WorkItem.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <returns></returns>
        public List<WorkItemJournalEntry> SelectWorkItemJournals(int workItemID)
        {
            List<WorkItemJournalEntry> rValue = new List<WorkItemJournalEntry>();

            using (var connection = new SQLiteConnection(BaseRepository.dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "SELECT *" +
                        " FROM vwActiveWorkItemJournal" +
                        " WHERE WorkItem_ID = @workItemID" +
                        " ORDER BY CreationDateTime ASC";
                    cmd.Parameters.AddWithValue("@workItemID", workItemID);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkItemJournalEntry journal = new WorkItemJournalEntry();
                            journal.WorkItem_ID = Convert.ToInt32(reader["WorkItem_ID"]);
                            if (reader["Header"].GetType() != typeof(DBNull))
                                journal.Title = (string)reader["Header"];
                            if (reader["Entry"].GetType() != typeof(DBNull))
                                journal.Entry = (string)reader["Entry"];
                            journal.CreationDateTime = DateTime.Parse(reader["CreationDateTime"].ToString());
                            if (reader["ModificationDateTime"].GetType() != typeof(DBNull))
                                journal.ModificationDateTime = DateTime.Parse(reader["ModificationDateTime"].ToString());

                            rValue.Add(journal);
                        }
                    }
                    connection.Close();
                }
            }
            return rValue;
        }
    
        /// <summary>
        /// Insert a WorkItemJournalEntry record into the database.
        /// </summary>
        /// <param name="journalEntry"></param>
        /// <returns></returns>
        public int InsertWorkItemJournalEntry(WorkItemJournalEntry journalEntry)
        {
            int workItemJournalEntryID = -1;
            DateTime creation = DateTime.Now;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "INSERT INTO WorkItemJournal (WorkItem_ID, Header, Entry, CreationDateTime) " +
                        "VALUES (@wID, @header, @entry, @creation)";
                    cmd.Parameters.AddWithValue("@wID", journalEntry.WorkItem_ID);
                    cmd.Parameters.AddWithValue("@header", journalEntry.Title);
                    cmd.Parameters.AddWithValue("@entry", journalEntry.Entry);
                    cmd.Parameters.AddWithValue("@creation", creation);
                    cmd.ExecuteNonQuery();

                    journalEntry.CreationDateTime = creation;

                    // Get the identity value (to return)
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    workItemJournalEntryID = (int)(Int64)cmd.ExecuteScalar();
                    journalEntry.WorkItemJournalID = workItemJournalEntryID;
                }
                connection.Close();
            }
            return workItemJournalEntryID;
        }

        /// <summary>
        /// Update a WorkItemJournalEntry record into the database.
        /// </summary>
        /// <param name="journalEntry"></param>
        public void UpdateWorkItemJournalEntry(WorkItemJournalEntry journalEntry)
        {
            DateTime modificationDate = DateTime.Now;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "UPDATE WorkItemJournal" +
                        " SET Header = @header," +
                        " Entry = @entry," +
                        " ModificationDateTime = @mod" +
                        " WHERE Journal_ID = @journalID";
                    cmd.Parameters.AddWithValue("@header", journalEntry.Title);
                    cmd.Parameters.AddWithValue("@entry", journalEntry.Entry);
                    cmd.Parameters.AddWithValue("@mod", modificationDate);
                    cmd.Parameters.AddWithValue("@journalID", journalEntry.WorkItemJournalID);
                    cmd.ExecuteNonQuery();

                    journalEntry.CreationDateTime = modificationDate;
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Delete all of the WorkItemJournalEntries, either physically or logically.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="logicalDelete"></param>
        public void DeleteAllWorkItemJournalEntries(int workItemID, bool logicalDelete)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    if (logicalDelete)
                    {
                        cmd.CommandText = "UPDATE WorkItemJournal" +
                            " SET DeletionDateTime = @deleteDate" +
                            " WHERE WorkItem_ID = @workItemID";
                        cmd.Parameters.AddWithValue("@deleteDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@workItemID", workItemID);
                    }
                    else
                    {
                        cmd.CommandText = "DELETE WorkItemJournal" +
                            " WHERE WorkItem_ID = @workItemID";
                        cmd.Parameters.AddWithValue("@workItemID", workItemID);
                    }
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Delete a WorkItemJournalEntry, either physically or logically.
        /// </summary>
        /// <param name="workItemJournalID"></param>
        /// <param name="logicalDelete"></param>
        public void DeleteWorkItemJournalEntry(int workItemJournalID, bool logicalDelete)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    if (logicalDelete)
                    {
                        cmd.CommandText = "UPDATE WorkItemJournal" +
                            " SET DeletionDateTime = @deleteDate" +
                            " WHERE Journal_ID = @workItemJournalID";
                        cmd.Parameters.AddWithValue("@deleteDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@workItemJournalID", workItemJournalID);
                    }
                    else
                    {
                        cmd.CommandText = "DELETE WorkItemJournal" +
                            " WHERE Journal_ID = @workItemJournalID";
                        cmd.Parameters.AddWithValue("@workItemJournalID", workItemJournalID);
                    }
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Get WorkItemJournal preferences.
        /// </summary>
        /// <returns></returns>
        Dictionary<PreferenceName, Preference> IWorkItemJournalRepository.GetWorkItemJournalPreferences()
        {
            return this.GetPreferences("WorkItemJournal");
        }
    }
}
