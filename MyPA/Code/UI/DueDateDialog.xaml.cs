using MyPA.Code.Data.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPA.Code.UI
{
    /// <summary>
    /// Interaction logic for DueDateDialog.xaml
    /// </summary>
    public partial class DueDateDialog : Window
    {
        public bool WasDialogSubmitted { get; set; } = false;

        /// <summary>
        /// This is here (as opposed to in the model) because it's only related to the UI, not the application.
        /// </summary>
        Border _originalBorder = new Border();

        public DueDateDialog(WorkItemDueDate widd)
        {
            InitializeComponent();
            ((DueDateViewModel)DataContext).OriginalData = widd;

            foreach (CalendarDateRange range in ((DueDateViewModel)DataContext).GetNonSelectableDatesAsCalendarDateRange())
            {
                CalendarSelection.BlackoutDates.Add(range);
            }

            DateTime originalDueDate = ((DueDateViewModel)DataContext).OriginalData.DueDateTime;
            CalendarSelection.DisplayDateStart = originalDueDate;
            CalendarSelection.SelectedDate = originalDueDate;

            if (((DueDateViewModel)DataContext).CloseAction == null)
                ((DueDateViewModel)DataContext).CloseAction = new Action(() => this.Close());
        }

        /// <summary>
        /// Select today's date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectToday(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(0, null);
        }

        /// <summary>
        /// Increment the calendar selection by 7 days.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select1Week(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(7, null);
        }

        /// <summary>
        /// Increment the calendar selection by 14 days.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select2Weeks(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(14, null);
        }

        /// <summary>
        /// Increment the calendar selection by 21 days.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select3Weeks(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(21, null);
        }

        /// <summary>
        /// Increment the calendar selection by 1 month.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select1Month(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(null, 1);
        }

        /// <summary>
        /// Increment the calendar selection by 12 months.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select1Year(object sender, RoutedEventArgs e)
        {
            IncrementDueDateSelection(null, 12);
        }

        /// <summary>
        /// Increment the current DueDate selection by the specified amount. 
        /// (Either numberOfDays or numberOfMonths can be selected, but not both).
        /// </summary>
        /// <param name="numberOfDays"></param>
        /// /// <param name="numberOfMonths"></param>
        private void IncrementDueDateSelection(int? numberOfDays = null, int? numberOfMonths = null)
        {
            // TODO put this method in VM?

            DateTime newDT = ((DueDateViewModel)DataContext).OriginalData.DueDateTime;

            if ((numberOfDays.HasValue) && (numberOfMonths.HasValue == false))
                newDT = newDT.AddDays(numberOfDays.Value);
            else if ((numberOfDays.HasValue == false) && (numberOfMonths.HasValue))
                newDT = newDT.AddMonths(numberOfMonths.Value);

            ((DueDateViewModel)DataContext).CurrentSelectedDate = newDT;
            // Read the value from the VM in case it was adjusted for black out days.
            newDT = ((DueDateViewModel)DataContext).CurrentSelectedDate;

            CalendarSelection.SelectedDate = newDT;
            CalendarSelection.DisplayDate = newDT;
        }

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

        private void CalendarSelection_GotMouseCapture(object sender, MouseEventArgs e)
        {
            UIElement originalElement = e.OriginalSource as UIElement;
            if (originalElement is CalendarDayButton || originalElement is CalendarItem)
            {
                originalElement.ReleaseMouseCapture();
            }
        }
    }
}
