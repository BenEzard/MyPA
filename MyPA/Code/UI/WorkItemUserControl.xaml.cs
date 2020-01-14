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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
