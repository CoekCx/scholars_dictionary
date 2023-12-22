using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Scholars_Dictionary.Services;
using Scholars_Dictionary.Models;
using Scholars_Dictionary.Enums;

namespace Scholars_Dictionary.Views
{
    /// <summary>
    /// Interaction logic for MyDictionary.xaml
    /// </summary>
    public partial class MyDictionary : Window
    {
        private List<LanguageEntry> allLanguageEntries;

        public MyDictionary()
        {
            InitializeComponent();

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;

            UpdateLanguageEntries();
            languageDataGrid.ItemsSource = allLanguageEntries;
        }

        #region Buttons
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LanguageEntry selectedEntry = (sender as FrameworkElement)?.DataContext as LanguageEntry;

            if (selectedEntry != null)
            {
                DeleteWord(selectedEntry.English);
            }
        }

        private void DeleteWord(string word)
        {
            WordCollectionService.RemoveWord(word);
            UpdateLanguageEntries();
            SearchFilterTable();
        }
        #endregion

        #region Helper
        private void UpdateLanguageEntries()
        {
            allLanguageEntries = new List<LanguageEntry>();

            foreach (var word in WordCollection.Collection[SupportedLanguages.ENGLISH].Keys)
            {
                try
                {
                    allLanguageEntries.Add(new LanguageEntry
                    {
                        English = WordCollection.Collection[SupportedLanguages.ENGLISH][word].Word,
                        Russian = WordCollection.Collection[SupportedLanguages.RUSSIAN][word].Word,
                        Spanish = WordCollection.Collection[SupportedLanguages.SPANISH][word].Word
                    });
                }
                catch (Exception) { }
            }
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
                languageDataGrid.ItemsSource = allLanguageEntries;
                return;
            }

            // Filter the data based on the English word column
            var filteredEntries = allLanguageEntries.Where(entry => entry.English.ToLower().Contains(searchTerm)).ToList();

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

        private void SearchTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate that the entered character is a letter
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; // Discard the non-letter character
            }
        }

        private void ClearSearch()
        {
            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
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
