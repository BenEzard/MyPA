using MyPA.Code.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MyPA.Code.Data.Services
{
    public class WorkItemRepository : BaseRepository, IWorkItemRepository
    {
        /// <summary>
        /// Get a collection of Work Items from the database.
        /// </summary>
        /// <param name="loadAgedDays">Used to selectively load Work Items based on the number of days since they've been put in a Closed state.
        /// Use -1 to load only active Work Items
        /// Use 0 to load only active Work Items + those Completed today. 
        /// Use 10 would give you active + any Work Items completed in the last 10 calendar days.</param>
        public List<WorkItem> GetWorkItems(int loadAgedDays)
        {
            // When this is called, first clear any content of the WorkItem collections.
            List<WorkItem> rValue = new List<WorkItem>();

            using (var connection = new SQLiteConnection(BaseRepository.dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "SELECT * FROM vwWorkItem ORDER BY DaysSinceCompletion ASC, DueDateTime ASC";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                          /*  int daysStale = Convert.ToInt32(reader["DaysSinceCompletion"]);

                            // Only create a WorkItem and add it to the model if DaysSinceCompletion <= loadAgedDays
                            if (daysStale <= loadAgedDays)
                            {*/
                                WorkItem workItem = new WorkItem();
                                int workItemID = Convert.ToInt32(reader["WorkItem_ID"]);
                                workItem.WorkItemID = workItemID;
                                workItem.Title = (string)reader["TaskTitle"];
                                if (reader["TaskDescription"].GetType() != typeof(DBNull))
                                    workItem.Description = (string)reader["TaskDescription"];
                                workItem.CreationDateTime = DateTime.Parse(reader["CreationDateTime"].ToString());
                                if (reader["ModificationDateTime"].GetType() != typeof(DBNull))
                                    workItem.ModificationDateTime = DateTime.Parse(reader["ModificationDateTime"].ToString());
                                if (reader["DeletionDateTime"].GetType() != typeof(DBNull))
                                    workItem.DeletionDateTime = DateTime.Parse(reader["DeletionDateTime"].ToString());

                                // --- WorkItemStatusEntry ----
                                WorkItemStatusEntry wise = new WorkItemStatusEntry();

                                wise.WorkItemStatusEntryID = Convert.ToInt32(reader["WorkItemStatusEntry_ID"]);
                                wise.WorkItemID = workItemID;
                                wise.WorkItemStatusID = Convert.ToInt32(reader["WorkItemStatus_ID"]);
                                wise.StatusLabel = (string)reader["wisStatusLabel"];
                                wise.CompletionAmount = Convert.ToInt32(reader["CompletionAmount"]);
                                wise.CreationDateTime = DateTime.Parse(reader["wisCreationDateTime"].ToString()); ;

                                DateTime? wiseModificationDateTime = null;
                                if (reader["wisModificationDateTime"] != DBNull.Value)
                                    wiseModificationDateTime = DateTime.Parse(reader["wisModificationDateTime"].ToString()); 
                                wise.ModificationDateTime = wiseModificationDateTime;

                                workItem.CurrentWorkItemStatusEntry = wise;

                                // -- Due Date
                                WorkItemDueDate dueDate = new WorkItemDueDate(Convert.ToInt32(reader["WorkItemDueDate_ID"]), workItemID);
                                dueDate.DueDateTime = DateTime.Parse(reader["DueDateTime"].ToString());
                                dueDate.CreationDateTime = DateTime.Parse(reader["DueDateCreationDateTime"].ToString());

                                if (reader["DueDateModificationDateTime"] != DBNull.Value)
                                    dueDate.ModificationDateTime = DateTime.Parse(reader["DueDateModificationDateTime"].ToString());

                                workItem.CurrentWorkItemDueDate = dueDate;

                                rValue.Add(workItem);
                           /* }
                            else
                            {
                                Console.WriteLine($"Not loading {(string)reader["TaskTitle"]} because it's stale: {daysStale}.");
                            }*/
                        }
                    }
                    connection.Close();
                }
            }

            return rValue;
        }

        /// <summary>
        /// Return all preferences used by the WorkItemViewModel.
        /// </summary>
        /// <returns></returns>
        Dictionary<PreferenceName, Preference> IWorkItemRepository.GetWorkItemPreferences()
        {
            return this.GetPreferences("WorkItem");
        }

        /// <summary>
        /// Insert WorkItem into the database, returning the WorkItemID.
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        public int InsertWorkItem(WorkItem workItem)
        {
            int workItemID = -1;
            DateTime creation = DateTime.Now;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "INSERT INTO WorkItem (TaskTitle, TaskDescription, CreationDateTime) " +
                        "VALUES (@title, @desc, @creation)";
                    cmd.Parameters.AddWithValue("@title", workItem.Title);
                    cmd.Parameters.AddWithValue("@desc", workItem.Description);
                    cmd.Parameters.AddWithValue("@creation", creation);
                    cmd.ExecuteNonQuery();

                    workItem.CreationDateTime = creation;

                    // Get the identity value (to return)
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    workItemID = (int)(Int64)cmd.ExecuteScalar();
                    workItem.WorkItemID = workItemID;
                }
                connection.Close();
            }
            return workItemID;
        }

        /// <summary>
        /// Insert a WorkItemDueDate into the database, returning the WorkItemDueDateID.
        /// </summary>
        /// <param name="workItemDueDate"></param>
        /// <returns></returns>
        public int InsertWorkItemDueDate(WorkItemDueDate workItemDueDate)
        {
            int workItemDueDateID = -1;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "INSERT INTO WorkItemDueDate (WorkItem_ID, DueDateTime, ChangeReason, CreationDateTime) " +
                        "VALUES (@workItemID, @dueDate, @changeReason, @creation)";
                    cmd.Parameters.AddWithValue("@workItemID", workItemDueDate.WorkItemID);
                    cmd.Parameters.AddWithValue("@dueDate", workItemDueDate.DueDateTime);
                    cmd.Parameters.AddWithValue("@changeReason", workItemDueDate.ChangeReason);
                    cmd.Parameters.AddWithValue("@creation", workItemDueDate.CreationDateTime);
                    cmd.ExecuteNonQuery();

                    // Get the identity value (to return)
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    workItemDueDateID = (int)(Int64)cmd.ExecuteScalar();
                    workItemDueDate.WorkItemDueDateID = workItemDueDateID;
                }
                connection.Close();
            }
            return workItemDueDateID;
        }

        /// <summary>
        /// Update a WorkItemDueDate into the database, returning the WorkItemDueDateID.
        /// </summary>
        /// <param name="workItemDueDate"></param>
        public void UpdateWorkItemDueDate(WorkItemDueDate workItemDueDate)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "UPDATE WorkItemDueDate " +
                        "SET DueDateTime = @dueDate, " +
                        "ChangeReason = @changeReason, " +
                        "CreationDateTime = @creation " +
                        "WHERE WorkItemDueDate_ID = @workItemDueDateID";
                    cmd.Parameters.AddWithValue("@workItemDueDateID", workItemDueDate.WorkItemID);
                    cmd.Parameters.AddWithValue("@dueDate", workItemDueDate.DueDateTime);
                    cmd.Parameters.AddWithValue("@changeReason", workItemDueDate.ChangeReason);
                    cmd.Parameters.AddWithValue("@creation", workItemDueDate.CreationDateTime);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Update the WorkItem.
        /// </summary>
        /// <param name="workItem"></param>
        public void UpdateWorkItem(WorkItem workItem)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "UPDATE WorkItem " +
                        "SET TaskTitle = @title, " +
                        "TaskDescription = @desc, " +
                        "ModificationDateTime = @modTime " +
                        "WHERE WorkItem_ID = @workItemID " +
                        "AND (TaskTitle <> @title " +
                        "OR TaskDescription <> @desc)";
                    cmd.Parameters.AddWithValue("@title", workItem.Title);
                    cmd.Parameters.AddWithValue("@desc", workItem.Description);
                    cmd.Parameters.AddWithValue("@workItemID", workItem.WorkItemID);
                    cmd.Parameters.AddWithValue("@modTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Get a collection of Work Item Statuses from the database.
        /// Includes both active and inactive.
        /// </summary>
        /// <returns></returns>
        public List<WorkItemStatus> GetWorkItemStatuses()
        {
            var rValue = new List<WorkItemStatus>();

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "SELECT * FROM vwWorkItemStatus";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var wis = new WorkItemStatus();
                            wis.WorkItemStatusID = Convert.ToInt32(reader["WorkItemStatus_ID"]);
                            wis.StatusLabel = (string)reader["StatusLabel"];
                            wis.IsConsideredActive = (bool)reader["IsConsideredActive"];
                            wis.IsDefault = (bool)reader["IsDefault"];
                            if (reader["DeletionDateTime"].GetType() != typeof(DBNull))
                                wis.DeletionDateTime = DateTime.Parse(reader["DeletionDateTime"].ToString());

                            rValue.Add(wis);
                        }
                    }

                    connection.Close();
                }
            }

            return rValue;
        }

        /// <summary>
        /// Inserts a database record for a WorkItemStatus.
        /// CompletionAmount is automatically set to 0.
        /// </summary>
        /// <param name="wise"></param>
        /// <returns>Returns the WorkItemStatusEntry ID on insert, or -1</returns>
        public int InsertWorkItemStatusEntry(BaseWorkItemStatusEntry wise)
        {
            int workItemStatusID = -1;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "INSERT INTO WorkItemStatusEntry (WorkItem_ID, WorkItemStatus_ID, CompletionAmount, CreationDateTime) " +
                        "VALUES (@workItemID, @statusID, @completionAmount, @creation)";
                    cmd.Parameters.AddWithValue("@workItemID", wise.WorkItemID);
                    cmd.Parameters.AddWithValue("@statusID", wise.WorkItemStatusID);
                    cmd.Parameters.AddWithValue("@completionAmount", wise.CompletionAmount);
                    cmd.Parameters.AddWithValue("@creation", wise.CreationDateTime);
                    cmd.ExecuteNonQuery();

                    // Get the identity value
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    workItemStatusID = (int)(Int64)cmd.ExecuteScalar();
                    wise.WorkItemStatusEntryID = workItemStatusID;

                    wise.PendingDBOperation = DBActionRequired.NONE;
                }
                connection.Close();
            }
            return workItemStatusID;
        }

        /// <summary>
        /// Update a WorkItemStatusEntry.
        /// </summary>
        /// <param name="wise"></param>
        public void UpdateWorkItemStatusEntry(BaseWorkItemStatusEntry wise)
        {
            wise.ModificationDateTime = DateTime.Now;
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "UPDATE WorkItemStatusEntry " +
                        "SET WorkItemStatus_ID = @workItemStatusID, " +
                        "CompletionAmount = @completionAmount, " +
                        "ModificationDateTime = @modTime " +
                        "WHERE WorkItem_ID = @workItemID";
                    cmd.Parameters.AddWithValue("@workItemStatusID", wise.WorkItemStatusID);
                    cmd.Parameters.AddWithValue("@completionAmount", wise.CompletionAmount);
                    cmd.Parameters.AddWithValue("@modTime", wise.ModificationDateTime);
                    cmd.Parameters.AddWithValue("@workItemID", wise.WorkItemID);
                    cmd.ExecuteNonQuery();

                    wise.PendingDBOperation = DBActionRequired.NONE;
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Get the user-created list of dates which are ineligible as DueDates.
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetIneligibleDueDates()
        {
            // When this is called, first clear any content of the WorkItem collections.
            List<DateTime> rValue = new List<DateTime>();

            using (var connection = new SQLiteConnection(BaseRepository.dbConnectionString))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();
                    cmd.CommandText = "SELECT IneligibleDate" +
                        " FROM IneligibleDueDate" +
                        " WHERE IneligibleDate >= @now" +
                        " ORDER BY IneligibleDate ASC";
                    cmd.Parameters.AddWithValue("@now", DateTime.Now.Date);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rValue.Add(DateTime.Parse(reader["IneligibleDate"].ToString()));
                        }
                    }
                    connection.Close();
                }
            }

            return rValue;
        }
    }
}
