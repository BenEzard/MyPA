using System;

namespace MyPA.Code.Data.Models
{
    public class WorkItemStatusFilter
    {
        public int WorkItemStatusID { get; set; }
        public string StatusLabel { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set {
                _isSelected = value;
                } 
        }


        public WorkItemStatusFilter(int workItemStatusID, string statusLabel)
        {
            WorkItemStatusID = workItemStatusID;
            StatusLabel = statusLabel;
            IsSelected = false;
        }
    }
}
