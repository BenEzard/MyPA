using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemStatusEntry : BaseWorkItemStatusEntry
    {
        public string StatusLabel{ get; set; }

        public WorkItemStatusEntry() { }

        /// <summary>
        /// Create a new WorkItemStatusEntry.
        /// Sets trhe CreationDateTime to NOW.
        /// </summary>
        /// <param name="workItemID"></param>
        /// <param name="workItemStatusID"></param>
        /// <param name="completionAmount"></param>
        /// <param name="label"></param>
        public WorkItemStatusEntry(int workItemID, int workItemStatusID, int completionAmount, string label)
        {
            WorkItemID = workItemID;
            WorkItemStatusID = workItemStatusID;
            CompletionAmount = completionAmount;
            CreationDateTime = DateTime.Now;
            StatusLabel = label;
        }

    }
}
