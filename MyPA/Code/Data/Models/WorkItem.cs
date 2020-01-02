using System;
using System.Collections.Generic;

namespace MyPA.Code.Data.Models
{
    public class WorkItem
    {
        public int? WorkItemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// A list of all associated WorkItemStatusEntries in reverse chronological order (newest is [0], oldest is [size-1])
        /// 
        /// </summary>
        public List<WorkItemStatusEntry> WorkItemStatusEntries = new List<WorkItemStatusEntry>(1);

        /// <summary>
        /// Flag to show whether or not all WorkItemStatusEntries have been loaded into the list.
        /// Should only be set from TODO.
        /// </summary>
        public bool AllWorkItemStatusEntriesLoaded { get; private set; }

        /// <summary>
        /// Get the Current WorkItemStatusEntry.
        /// </summary>
        public WorkItemStatusEntry CurrentWorkItemStatusEntry
        {
            get
            {
                return WorkItemStatusEntries[0];
            }
        }

        public DateTime CreationDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
        public DateTime? DeletionDateTime { get; set; }

        //public WorkItemStatusHistory CurrentStatus { get;set; }
    }
}
