using MyPA.Code.UI;

namespace MyPA.Code.Data.Actions
{
    public class MoveVerticalWorkItemJournalSplitAction
    {
        public SplitSetting VerticalSplit;

        public MoveVerticalWorkItemJournalSplitAction()
        {
        }

        public MoveVerticalWorkItemJournalSplitAction(SplitSetting verticalSplit)
        {
            VerticalSplit = verticalSplit;
        }
    }
}
