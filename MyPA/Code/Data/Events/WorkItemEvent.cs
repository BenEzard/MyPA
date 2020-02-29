using MyPA.Code.Data.Actions;
using MyPA.Code.UI;

namespace MyPA.Code.Data.Events
{
    public partial class WorkItemEvent
    {
        private WorkItemJournalEntry _workItemJournalEntry;
        public WorkItemJournalEntry WorkItemJournalEntry
        {
            get => _workItemJournalEntry;
        }

        public WorkItemJournalAction EventAction;

        public SplitSetting VerticalSplit;

        public WorkItemEvent(WorkItemJournalAction action, SplitSetting verticalSplitSetting)
        {
            EventAction = action;
            VerticalSplit = verticalSplitSetting;
        }

        public WorkItemEvent(WorkItemJournalAction action)
        {
            EventAction = action;
        }
    }
}
