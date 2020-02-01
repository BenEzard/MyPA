using MyPA.Code;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Does this need to be elsewhere (not in code-behind)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((ApplicationViewModel)DataContext).SaveWindowLocation(this.Height, this.Width, this.Top, this.Left);
        }

        /// <summary>
        /// Does this need to be elsewhere (not in code-behind)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = ((ApplicationViewModel)DataContext).ApplicationPositionLeft;
            this.Top = ((ApplicationViewModel)DataContext).ApplicationPositionTop;
            this.Width = ((ApplicationViewModel)DataContext).ApplicationWidth;
            this.Height = ((ApplicationViewModel)DataContext).ApplicationHeight;
        }
    }
}
