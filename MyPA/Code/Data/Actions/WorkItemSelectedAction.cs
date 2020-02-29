using MyPA.Code.Data.Models;

namespace MyPA.Code.Data.Actions
{
    public class WorkItemSelectedAction
    {
        public WorkItem WorkItem {get; set; }

        public WorkItemSelectedAction(WorkItem workItem)
        {
            WorkItem = workItem;
        }
    }
}
