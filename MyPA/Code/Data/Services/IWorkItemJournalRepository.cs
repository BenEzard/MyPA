using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    internal interface IWorkItemJournalRepository
    {

        Dictionary<PreferenceName, Preference> GetWorkItemJournalPreferences();

        /// <summary>
        /// Get all of the WorkItemJournal entries for a specified WorkItem ID.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <returns></returns>
        List<WorkItemJournalEntry> SelectWorkItemJournals(int workItemID);

        /// <summary>
        /// Insert a WorkItemJournal entry.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns></returns>
        int InsertWorkItemJournalEntry(WorkItemJournalEntry journal);

        void UpdateWorkItemJournalEntry(WorkItemJournalEntry journal);

        /// <summary>
        /// Delete all of the WorkItemJournalEntries for a given WorkItem.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="logicalDelete"></param>
        void DeleteAllWorkItemJournalEntries(int workItemID, bool logicalDelete);

        /// <summary>
        /// Delete a single WorkItemJournalEntry.
        /// </summary>
        /// <param name="workItemJournalID"></param>
        /// <param name="logicalDelete"></param>
        void DeleteWorkItemJournalEntry(int workItemJournalID, bool logicalDelete);
    }
}