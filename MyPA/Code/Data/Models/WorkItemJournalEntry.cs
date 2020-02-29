using MyPA.Code.Data.Models;
using MyPA.Code.UI.Util;
using System;

namespace MyPA.Code
{
    public class WorkItemJournalEntry : BaseDBObject
    {
        public int? WorkItemJournalID { get; set; } = -1;

        public int WorkItem_ID { get; set; }

        public string Title { get; set; }
        public string Entry { get; set; }

        public WorkItemJournalEntry() { }

        public WorkItemJournalEntry(int workItemID)
        {
            WorkItem_ID = workItemID;
        }

        public WorkItemJournalEntry(string title, string entry)
        {
            Title = title;
            Entry = entry;
        }

        public WorkItemJournalEntry(int journalID, string title, string entry, DateTime? creationDateTime, DateTime? modificationDateTime)
        {
            WorkItemJournalID = journalID;
            Title = title;
            Entry = entry;
            CreationDateTime = creationDateTime;
            ModificationDateTime = modificationDateTime;
        }

        public WorkItemJournalEntry(string title, string entry, DateTime? creationDateTime, DateTime? modificationDateTime, DateTime? deletionDateTime)
        {
            Title = title;
            Entry = entry;
            if (creationDateTime.HasValue)
                CreationDateTime = creationDateTime.Value;
            if (modificationDateTime.HasValue)
            ModificationDateTime = modificationDateTime.Value;
            if (deletionDateTime.HasValue)
            DeletionDateTime = deletionDateTime.Value;
        }

        // <summary>
        /// Generates a string which describes how long since the WorkItem was created.
        /// Bound to WorkItemUserControl.
        /// </summary>
        public string TimeSinceCreationString
        {
            get
            {
                string rValue = "";
                if (CreationDateTime.HasValue == false)
                    rValue = DateMethods.GenerateDateDifferenceLabel(DateTime.Now, DateTime.Now, true);
                else
                    rValue = DateMethods.GenerateDateDifferenceLabel(CreationDateTime.Value, DateTime.Now, true);
                return rValue;
            }
        }

    }
}
