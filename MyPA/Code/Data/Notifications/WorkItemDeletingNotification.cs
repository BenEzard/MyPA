using MyPA.Code.Data.Models;

namespace MyPA.Code.Data.Actions
{
    public class WorkItemDeletingNotification : BaseNotification
    {
        public WorkItem WorkItem {get; set; }
        public bool LogicalDelete { get; }

        public WorkItemDeletingNotification() { }

        public WorkItemDeletingNotification(WorkItem workItem, bool logical)
        {
            WorkItem = workItem;
            LogicalDelete = logical;
        }
    }
}
