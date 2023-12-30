using Scholars_Dictionary.Services;
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
        public static double NewLeft { get; set; }
        public static double NewTop { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            WordCollectionService.LoadCollection();
            WordCollectionService.SafeCheckCollection();
            UserCollectionService.LoadCollections();
        }

        // Menu Buttons
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            var window = new VocabularyBuilder();

            // Set the position of the new window
            window.Left = Left;
            window.Top = Top;

            Hide();
            window.ShowDialog();

            // Show the current window at its original position
            Left = NewLeft;
            Top = NewTop;
            Show();
        }

        private void buttonMyCollections_Click(object sender, RoutedEventArgs e)
        {
            var window = new MyCollections();

            // Set the position of the new window
            window.Left = Left;
            window.Top = Top;

            Hide();
            window.ShowDialog();

            // Show the current window at its original position
            Left = NewLeft;
            Top = NewTop;
            try
            {
                Show();
            }
            catch(Exception) { }
        }

        private void buttonMyDictionary_Click(object sender, RoutedEventArgs e)
        {
            var window = new MyDictionary();

            // Set the position of the new window
            window.Left = Left;
            window.Top = Top;

            Hide();
            window.ShowDialog();

            // Show the current window at its original position
            Left = NewLeft;
            Top = NewTop;
            Show();
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
