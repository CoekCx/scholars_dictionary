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
        private bool _isRussianChecked;
        private bool _isSpanishChecked;

        public bool IsRussianChecked
        {
            get => _isRussianChecked;
            set => _isRussianChecked = value;
        }
        public bool IsSpanishChecked
        {
            get => _isSpanishChecked;
            set => _isSpanishChecked = value;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Language Selection
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Content.ToString().Contains("Russian"))
                {
                    IsRussianChecked = true;
                }
                else if (checkBox.Content.ToString().Contains("Spanish"))
                {
                    IsSpanishChecked = true;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Content.ToString().Contains("Russian"))
                {
                    IsRussianChecked = false;
                }
                else if (checkBox.Content.ToString().Contains("Spanish"))
                {
                    IsSpanishChecked = false;
                }
            }
        }

        // Menu Buttons
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            return;
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
