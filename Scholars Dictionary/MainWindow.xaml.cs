using Scholars_Dictionary.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scholars_Dictionary
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

        // Menu Buttons
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            var window = new VocabularyBuilder();
            Hide();
            window.ShowDialog();
            try
            {
                Show();
            }
            catch (Exception) { }
        }

        // Top Bar
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
