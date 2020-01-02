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

                                // --- WorkItemStatus ----
                                DateTime? wiseModificationDateTime = null;
                                if (reader["wisModificationDateTime"] != DBNull.Value)
                                    wiseModificationDateTime = DateTime.Parse(reader["wisModificationDateTime"].ToString());

                                WorkItemStatusEntry wise = new WorkItemStatusEntry();
                                wise.WorkItemStatusEntryId = Convert.ToInt32(reader["WorkItemStatusEntry_ID"]);
                                wise.WorkItemId = workItemID;
                                wise.WorkItemStatusId = Convert.ToInt32(reader["WorkItemStatus_ID"]);
                                wise.CompletionAmount = Convert.ToInt32(reader["CompletionAmount"]);
                                wise.CreationDateTime = DateTime.Parse(reader["wisCreationDateTime"].ToString()); ;
                                wise.ModificationDateTime = wiseModificationDateTime;
                                workItem.WorkItemStatusEntries.Add(wise);

                                /*                                
                                if (reader["DueDateTime"] != DBNull.Value)
                                    wi.DueDate = DateTime.Parse(reader["DueDateTime"].ToString());

                                if (reader["DueDateCreationDateTime"] != DBNull.Value)
                                    wi.Meta.DueDateUpdateDateTime = DateTime.Parse(reader["DueDateCreationDateTime"].ToString());

                                wi.Status = reader["wisStatusLabel"].ToString();
                                wi.Meta.StatusUpdateDateTime = DateTime.Parse(reader["wisStatusDateTime"].ToString());
                                wi.IsConsideredActive = Boolean.Parse(reader["wisIsConsideredActive"].ToString());

                                int statusID = Convert.ToInt32(reader["wisStatus_ID"]);
                                wi.workItemStatus = GetWorkItemStatus(statusID);

                                Console.WriteLine($"Loading {(string)reader["TaskTitle"]}");
                                _model.AddWorkItem(wi, false, false);
                                */

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

        public void AddWorkItem(WorkItem workItem)
        {
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
    }
}
