namespace MyPA.Code.UI
{
    public class UITab
    {
        public string ItemTabName { get; set; }
        public int TabIndex { get; set; }

        public UITab(string itemTabName, int tabIndex)
        {
            ItemTabName = itemTabName;
            TabIndex = tabIndex;
        }
    }
}
