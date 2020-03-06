using MyPA.Code.Data.Models;

namespace MyPA.Code.Data.Actions
{
    public class WorkItemSelectedNotification : BaseNotification
    {
        public WorkItem WorkItem {get; set; }

        public WorkItemSelectedNotification(WorkItem workItem)
        {
            WorkItem = workItem;
        }
    }
}
