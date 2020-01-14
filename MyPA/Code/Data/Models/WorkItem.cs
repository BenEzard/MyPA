using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItem : BaseWorkItem
    {
        private WorkItemStatusEntry _currentWISE;



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


        public double DaysSinceCreation { 
            get
            {
                return (DateTime.Now - CreationDateTime).TotalDays;
            }
        }

    }
}
