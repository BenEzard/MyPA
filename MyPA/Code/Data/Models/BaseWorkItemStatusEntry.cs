using System;

namespace MyPA.Code.Data.Models
{
    /// <summary>
    /// The WorkItemStatusEntry represents an record-entry showing the status of a WorkItem.
    /// </summary>
    public class BaseWorkItemStatusEntry : BaseDBObject
    {
        /// <summary>
        /// The primary key for the WorkItemStatusEntry table.
        /// </summary>
        public int WorkItemStatusEntryID { get; set; }
        public int WorkItemID { get; set; }
        public int WorkItemStatusID { get; set; }
        public int CompletionAmount { get; set; }

        public BaseWorkItemStatusEntry() { }

        /// <summary>
        /// /// Create a new WorkItemStatusEntry with the given workItemID, workItemStatusID and completionAmount.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="workItemStatusID"></param>
        /// <param name="completionAmount"></param>
        public BaseWorkItemStatusEntry(int workItemID, int workItemStatusID, int completionAmount)
        {
            WorkItemID = workItemID;
            WorkItemStatusID = workItemStatusID;
            CompletionAmount = completionAmount;
            CreationDateTime = DateTime.Now;
        }
    }
}
