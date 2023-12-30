using Scholars_Dictionary.Models;
using Scholars_Dictionary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scholars_Dictionary.Views
{
    /// <summary>
    /// Interaction logic for MyCollections.xaml
    /// </summary>
    public partial class MyCollections : Window
    {
        private List<UserCollectionTableModel> allUserCollections;

        public MyCollections()
        {
            InitializeComponent();

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;

            UpdateUserCollectionEntries();
            languageDataGrid.ItemsSource = allUserCollections;
        }

        #region Buttons
        private void buttonNewCollection_Click(object sender, RoutedEventArgs e)
        {
            var window = new CollectionView();

            // Set the position of the new window
            window.Left = Left + 316.5;
            window.Top = Top + 75;

            window.ShowDialog();
            UpdateUserCollectionEntries();
            languageDataGrid.ItemsSource = allUserCollections;
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCollectionTableModel selectedEntry = (sender as FrameworkElement)?.DataContext as UserCollectionTableModel;

            if (selectedEntry != null)
            {
                EditCollection(selectedEntry.Name);
            }
        }

        private void EditCollection(string collectionName)
        {
            var userCollection = UserCollectionService.GetCollectionByName(collectionName);

            var window = new CollectionView(userCollection);

            // Set the position of the new window
            window.Left = Left + 316.5;
            window.Top = Top + 75;

            window.ShowDialog();
            UpdateUserCollectionEntries();
            languageDataGrid.ItemsSource = allUserCollections;
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCollectionTableModel selectedEntry = (sender as FrameworkElement)?.DataContext as UserCollectionTableModel;

            if (selectedEntry != null)
            {
                DeleteCollection(selectedEntry.Name);
            }
        }

        private void DeleteCollection(string collectionName)
        {
            UserCollectionService.RemoveCollection(collectionName: collectionName);
            UpdateUserCollectionEntries();
            SearchFilterTable();
        }
        #endregion

        #region Search
        private void SearchTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchFilterTable();
        }

        private void SearchFilterTable()
        {
            string searchTerm = searchTxtBox.Text.ToLower();

            if (searchTerm == "search...")
            {
                languageDataGrid.ItemsSource = allUserCollections;
                return;
            }

            // Filter the data based on the English word column
            var filteredEntries = allUserCollections.Where(entry => entry.Name.ToLower().Contains(searchTerm)).ToList();

            // Update the DataGrid with the filtered data
            languageDataGrid.ItemsSource = filteredEntries;
        }

        private void SearchGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;

            if (searchBox.Text == "Search...")
            {
                searchBox.Text = string.Empty;
                searchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                ClearSearch();
            }
        }

        private void ClearSearch()
        {
            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
        }
        #endregion

        #region Helper
        private void UpdateUserCollectionEntries()
        {
            allUserCollections = new List<UserCollectionTableModel>();

            foreach (var collection in UserCollectionService.Collections)
            {
                var nextEntnry = new UserCollectionTableModel()
                {
                    Name = collection.Name,
                    Count = collection.Collection[Enums.SupportedLanguages.ENGLISH].Count(),
                };

                allUserCollections.Add(nextEntnry);
            }
        }
        #endregion

        #region Top Bar
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NewLeft = Left;
            MainWindow.NewTop = Top;
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion
    }
}
