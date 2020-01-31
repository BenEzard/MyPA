using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemDueDate : BaseDBObject
    {
        public int WorkItemDueDateID { get; set; }
        public int WorkItemID { get; set; }
        public DateTime DueDateTime { get; set; }
        public string ChangeReason { get; set; }

        public WorkItemDueDate(int workItemID)
        {
            WorkItemID = workItemID;
        }

        public WorkItemDueDate(DateTime dueDate)
        {
            DueDateTime = dueDate;
        }

        public WorkItemDueDate(int workItemDueDateID, int workItemID)
        {
            WorkItemDueDateID = workItemDueDateID;
            WorkItemID = workItemID;
        }

        public WorkItemDueDate(int workItemID, DateTime dueDate, string changeReason)
        {
            WorkItemID = workItemID;
            DueDateTime = dueDate;
            ChangeReason = changeReason;
            CreationDateTime = DateTime.Now;
        }

        public WorkItemDueDate(int workItemID, DateTime dueDate, string changeReason, DateTime creationDate, DateTime? modDate)
        {
            WorkItemID = workItemID;
            DueDateTime = dueDate;
            ChangeReason = changeReason;
            CreationDateTime = creationDate;
            if (modDate.HasValue)
                ModificationDateTime = modDate.Value;
        }
    }
}
