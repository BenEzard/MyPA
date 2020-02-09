using MyPA.Code.UI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyPA.Code.Data.Models
{
    public class WorkItem : BaseWorkItem, INotifyPropertyChanged
    {
        /// <summary>
        /// The list of currently loaded WorkItemStatusEntries.
        /// Note that this will normally be just the latest; unless a a status/completion value changes, in which case these will be
        /// added to index 0 of the list.
        /// A future update will enable the full loading of the status-change history (when requested), to allow graphs etc.
        /// </summary>
        private List<WorkItemStatusEntry> _workItemStatusEntries = new List<WorkItemStatusEntry>(0);

        private List<WorkItemDueDate> _workItemDueDates = new List<WorkItemDueDate>(0);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Add a WorkItemStatusEntry to the list.
        /// By convention new items should be added at position 0
        /// </summary>
        /// <param name="entry"></param>
        public void AddWorkItemStatusEntry(WorkItemStatusEntry entry)
        {
            _workItemStatusEntries.Insert(0, entry);
        }

        /// <summary>
        /// Returns the current WorkItemStatusEntry.
        /// </summary>
        /// <returns></returns>
        public WorkItemStatusEntry GetLastWorkItemStatusEntry()
        {
            if (_workItemStatusEntries.Count > 0)
                return _workItemStatusEntries[0];
            else
                return null;
        }

        /// <summary>
        /// Return the number of WorkItemStatusEntries loaded for this WorkItem.
        /// </summary>
        public int GetWorkItemStatusEntryCount {
           get => _workItemStatusEntries.Count;
        }

        public WorkItemStatusEntry CurrentWorkItemStatusEntry
        {
            get
            {
                WorkItemStatusEntry rValue;
                if (_workItemStatusEntries.Count == 0)
                {
                    rValue = null;
                }
                else 
                    rValue = _workItemStatusEntries[0];

                return rValue;
            }
            set
            {
                AddWorkItemStatusEntry(value);
                OnPropertyChanged("");
            }
        }

        public List<WorkItemStatusEntry> GetWorkItemStatusEntries() => _workItemStatusEntries;

        public void AddWorkItemDueDate(WorkItemDueDate entry)
        {
            _workItemDueDates.Insert(0, entry);
        }

        public WorkItemDueDate CurrentWorkItemDueDate
        {
            get
            {
                WorkItemDueDate rValue;
                if (_workItemDueDates.Count == 0)
                {
                    rValue = null;
                }
                else
                    rValue = _workItemDueDates[0];

                return rValue;
            }
            set
            {
                AddWorkItemDueDate(value);
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Generates a string which describes how long since the WorkItem was created.
        /// Bound to WorkItemUserControl.
        /// </summary>
        public string TimeSinceCreationString { 
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

        public WorkItem()
        {
            CreationDateTime = DateTime.Now;
        }

        internal int WorkItemDueDateCount() => _workItemDueDates.Count;
    }
}
