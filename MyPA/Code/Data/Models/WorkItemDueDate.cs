using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemDueDate
    {
        public int WorkItemDueDateId { get; set; }
        public int WorkItemId { get; set; }
        public DateTime DueDateTime { get; set; }
        public string ChangeReason { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ModificationDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }
    }
}
