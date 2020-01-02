using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemStatus
    {
        public int WorkItemStatusID { get; set; }
        public string StatusLabel { get; set; }
        public bool IsConsideredActive { get; set; }
        public bool IsDefault { get; set; }

        public DateTime CreationDateTime { get; set; }
        public DateTime DeletionDateTime { get; set; }

    }
}
