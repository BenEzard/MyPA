using System;

namespace MyPA.Code.Data.Models
{
    /// <summary>
    /// The WorkItemStatusEntry represents an record-entry showing the status of a WorkItem.
    /// </summary>
    public class WorkItemStatusEntry
    {
        /// <summary>
        /// The primary key for the WorkItemStatusEntry table.
        /// </summary>
        public int WorkItemStatusEntryId { get; set; }
        public int WorkItemId { get; set; }
        public int WorkItemStatusId { get; set; }
        public int CompletionAmount { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
        public DateTime? DeletionDateTime { get; set; }
    }
}
