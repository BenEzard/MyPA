using MyPA.Code.Data.Actions;
using MyPA.Code.UI;

namespace MyPA.Code.Data.Events
{
    public partial class WorkItemJournalEvent
    {
        private WorkItemJournalEntry _workItemJournalEntry;
        public WorkItemJournalEntry WorkItemJournalEntry
        {
            get => _workItemJournalEntry;
        }

        public WorkItemJournalAction EventAction;

        public SplitSetting VerticalSplit;

        public WorkItemJournalEvent(WorkItemJournalAction action, SplitSetting verticalSplitSetting)
        {
            EventAction = action;
            VerticalSplit = verticalSplitSetting;
        }

        public WorkItemJournalEvent(WorkItemJournalAction action)
        {
            EventAction = action;
        }

    }
}
