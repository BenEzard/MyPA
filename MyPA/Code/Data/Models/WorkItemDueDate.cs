using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemDueDate
    {
        public int WorkItemDueDateID { get; set; }
        public int WorkItemID { get; set; }
        public DateTime DueDateTime { get; set; }
        public string ChangeReason { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }

        public WorkItemDueDate(int workItemID, DateTime dueDate, string changeReason)
        {
            WorkItemID = workItemID;
            DueDateTime = dueDate;
            ChangeReason = changeReason;
            CreationDateTime = DateTime.Now;
        }
    }
}
