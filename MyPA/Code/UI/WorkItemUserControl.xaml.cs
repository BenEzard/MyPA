using MyPA.Code.Data.Actions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPA.Code.UI
{
    /// <summary>
    /// Interaction logic for WorkItemUserControl.xaml
    /// </summary>
    public partial class WorkItemUserControl : UserControl
    {
        public WorkItemUserControl()
        {
            InitializeComponent();
            ((WorkItemViewModel)DataContext).ModelEvent += ModelEventListener;
            RegisterUITabs();
        }

        /// <summary>
        /// Register all of the WorkItem UI tabs with the model.
        /// </summary>
        private void RegisterUITabs()
        {
            int i = 0;
            foreach (TabItem item in WorkItemTabControl.Items)
            {
                ((WorkItemViewModel)DataContext).RegisterUITab(item.Name);
                i++;
            }
        }

        /// <summary>
        /// This is here (as opposed to in the model) because it's only related to the UI, not the application.
        /// </summary>
        Border _originalBorder = new Border();

        /// <summary>
        /// To highlight the selected control, add a left border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlGainsFocus(object sender, RoutedEventArgs e)
        {
            Control c = e.Source as Control;
            _originalBorder.BorderThickness = c.BorderThickness;
            _originalBorder.BorderBrush = c.BorderBrush;

            c.BorderBrush = Brushes.DodgerBlue;
            c.BorderThickness = new Thickness(4, 0, 0, 0);
        }

        /// <summary>
        /// Reset the formerly-highlighted control by restoring it's borders.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlLosesFocus(object sender, RoutedEventArgs e)
        {
            Control c = e.Source as Control;
            c.BorderBrush = _originalBorder.BorderBrush;
            c.BorderThickness = _originalBorder.BorderThickness;
        }

        private void DueDateButton_Click(object sender, RoutedEventArgs e)
        {
            var dd = new DueDateDialog(((WorkItemViewModel)DataContext).SelectedWorkItem.CurrentWorkItemDueDate);
            dd.ShowDialog();
        }

        /// <summary>
        /// Catch UI events sent from the Model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="action"></param>
        public void ModelEventListener(Object sender, BaseNotification action)
        {
            if (action is WorkItemCreatingNotification)
            {
                Console.WriteLine("WorkItemCreatingAction in usercontrol");
                SelectedTaskTitleField.Focus();
            }
        }

    }
}
