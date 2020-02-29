using System.Windows.Input;

namespace MyPA.Code.Data
{
    public class WorkItemSaveCommand
    {
        public string ButtonText { get; set; } = null;
        public string ButtonImagePath { get; set; } = null;
        public ICommand CommandAction { get; set; } = null;
    }
}
