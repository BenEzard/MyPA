using MyPA.Code.UI;

namespace MyPA.Code.Data.Actions
{
    public class MoveVerticalWorkItemJournalSplitNotification : BaseNotification
    {
        public SplitSetting VerticalSplit;

        public MoveVerticalWorkItemJournalSplitNotification()
        {
        }

        public MoveVerticalWorkItemJournalSplitNotification(SplitSetting verticalSplit)
        {
            VerticalSplit = verticalSplit;
        }
    }
}
