using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    internal interface IWorkItemJournalRepository
    {
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
    }
}