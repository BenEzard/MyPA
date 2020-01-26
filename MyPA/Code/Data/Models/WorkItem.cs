using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyPA.Code.Data.Models
{
    public class WorkItem : BaseWorkItem, INotifyPropertyChanged
    {
        private WorkItemStatusEntry _currentWISE;

        private List<WorkItemStatusEntry> _workItemStatusEntries = new List<WorkItemStatusEntry>(1);
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

        public WorkItemStatusEntry GetLastWorkItemStatusEntry()
        {
            return _workItemStatusEntries[0];
        }

        public WorkItemStatusEntry LastWorkItemStatusEntry
        {
            get {
                return _workItemStatusEntries[0];
            }
            set
            {
                AddWorkItemStatusEntry(value);
                OnPropertyChanged("");
            }
        }


        /// <summary>
        /// Get the Current WorkItemStatusEntry.
        /// </summary>
        public WorkItemStatusEntry CurrentWorkItemStatusEntry
        {
            get
            {
                return _currentWISE;
            }
            set
            {
                _currentWISE = value;
            }
        }

/*        private int _completionAmount;
        public int CompletionAmount
        {
            get {
                return _completionAmount;
            }
            set
            {
                _completionAmount = value;
            }
        }*/


        public double DaysSinceCreation { 
            get
            {
                return (DateTime.Now - CreationDateTime).TotalDays;
            }
        }


    }
}
