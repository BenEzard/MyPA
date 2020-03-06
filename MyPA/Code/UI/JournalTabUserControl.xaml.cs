using MyPA.Code.Data.Actions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyPA.Code.UI
{
    /// <summary>
    /// Interaction logic for JournalTabUserControl.xaml
    /// </summary>
    public partial class JournalTabUserControl : UserControl
    {
        private const string TAB_NAME = "TabTaskJournal";

        public JournalTabUserControl()
        {
            InitializeComponent();
            ((WorkItemJournalViewModel)DataContext).ModelEvent += ModelEventListener;
        }

        private void VerticalGridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (((WorkItemJournalViewModel)DataContext).VerticalSplit)
            {
                case SplitSetting.EQUAL_SPLIT:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;
                case SplitSetting.LEFT_EXPANDED:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Pixel);
                    break;
                case SplitSetting.RIGHT_EXPANDED:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;
            }
        }

        /// <summary>
        /// Receives events from the WorkItemJournalViewModel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        public void ModelEventListener(Object sender, BaseNotification notification)
        {
            if (notification is WorkItemJournalCreatingNotification)
            {
                Messenger.Default.Send(new WorkItemSelectTabAction(TAB_NAME));
                MoveVerticalSplit(SplitSetting.RIGHT_EXPANDED);
                JournalTitleField.Focus();
            }
            else if (notification is MoveVerticalWorkItemJournalSplitNotification)
            {
                var notificationDetails = (MoveVerticalWorkItemJournalSplitNotification)notification;
                MoveVerticalSplit(notificationDetails.VerticalSplit);
            }
        }

        private void MoveVerticalSplit(SplitSetting verticalSplit)
        {
            switch (verticalSplit)
            {
                case SplitSetting.EQUAL_SPLIT:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;
                case SplitSetting.LEFT_EXPANDED:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Pixel);
                    break;
                case SplitSetting.RIGHT_EXPANDED:
                    GridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Pixel);
                    GridContent.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;
            }
        }

        /// <summary>
        /// Event fired whenever the WorkItem Journal Entry (Title or Body) is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Journal_TextChanged(object sender, RoutedEventArgs e)
        {
            ((WorkItemJournalViewModel)DataContext).UpdateJournalEntry();
        }
    }
}
