using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemStatusEntry : BaseWorkItemStatusEntry
    {
        public string StatusLabel{ get; set; }

        public WorkItemStatusEntry() { }

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
