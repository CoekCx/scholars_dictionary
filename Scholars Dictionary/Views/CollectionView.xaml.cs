using Scholars_Dictionary.Enums;
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
    /// Interaction logic for CollectionView.xaml
    /// </summary>
    public partial class CollectionView : Window
    {
        private List<WordTableModel> allWordTableModels = new List<WordTableModel>();
        private List<string> allCollectionNames = new List<string>();
        private bool IsNewCollection;
        private UserCollection existingUserCollection = null;

        public CollectionView()
        {
            InitializeComponent();
            DataContext = this;

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;

            nameTxtBox.Text = "Enter Collection Name...";
            nameTxtBox.Foreground = Brushes.Gray;

            UpdateWordTableModelEntries();
            UpdateCollectionNamesEntries();

            wordDataGrid.ItemsSource = allWordTableModels;

            IsNewCollection = true;
        }

        public CollectionView(UserCollection userCollection)
        {
            InitializeComponent();
            DataContext = this;

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;

            nameTxtBox.Text = userCollection.Name;

            UpdateWordTableModelEntries();
            UpdateCollectionNamesEntries();

            foreach(string word in userCollection.Collection[SupportedLanguages.ENGLISH].Keys)
            {
                var model = allWordTableModels.FirstOrDefault(x => x.Word.Equals(word));
                model.IsWordSelected = true;
            }

            wordDataGrid.ItemsSource = allWordTableModels;

            var alreadySelectedWords = userCollection.Collection[SupportedLanguages.ENGLISH].Select(x => x.Key).ToList();
            CheckCheckBoxesForWords(alreadySelectedWords);

            IsNewCollection = false;
            existingUserCollection = userCollection;
        }

        #region Buttons
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if(!Validate())
            {
                return;
            }

            var newCollection = new UserCollection()
            {
                Name = nameTxtBox.Text,
                Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>()
            };

            var selectedWords = new List<string>();

            foreach (var item in wordDataGrid.ItemsSource)
            {
                if (item is WordTableModel itemType)
                {
                    if (((WordTableModel)item).IsWordSelected)
                    {
                        selectedWords.Add(itemType.Word);
                    }
                }
            }

            UserCollectionService.FillWords(newCollection, selectedWords);
            UserCollectionService.RemoveCollection(existingUserCollection);
            UserCollectionService.AddCollection(newCollection);

            Close();
        }

        private bool Validate()
        {
            if (IsNewCollection && allCollectionNames.Contains(nameTxtBox.Text))
            {
                return false;
            }
            else if (!IsNewCollection && allCollectionNames.Contains(nameTxtBox.Text) && !nameTxtBox.Text.Equals(existingUserCollection.Name))
            {
                return false;
            }

            if (String.IsNullOrEmpty(nameTxtBox.Text) || nameTxtBox.Text.Equals("Enter Collection Name..."))
            {
                return false;
            }

            return true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Name
        private void NameGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox nameTxtBox = (TextBox)sender;

            if (nameTxtBox.Text == "Enter Collection Name...")
            {
                nameTxtBox.Text = string.Empty;
                nameTxtBox.Foreground = Brushes.Black;
            }
        }

        private void NameLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                nameTxtBox.Text = "Enter Collection Name...";
                nameTxtBox.Foreground = Brushes.Gray;
            }
        }

        private void NameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void NameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNewCollection && allCollectionNames.Contains(nameTxtBox.Text))
            {
                nameBorder.BorderBrush = Brushes.Red;
                nameBorder.BorderThickness = new Thickness(1.5);

                nameErrorLabel.Content = "A collection with that name already exists!";
            }
            else if (!IsNewCollection && allCollectionNames.Contains(nameTxtBox.Text) && !nameTxtBox.Text.Equals(existingUserCollection.Name))
            {
                nameTxtBox.BorderBrush = Brushes.Red;
                nameBorder.BorderThickness = new Thickness(1.5);

                nameErrorLabel.Content = "A collection with that name already exists!";
            }
            else
            {
                nameBorder.BorderBrush = Brushes.Black;
                nameBorder.BorderThickness = new Thickness(1);

                nameErrorLabel.Content = "";
            }
        }
        #endregion

        #region Search
        private void SearchFilterTable()
        {
            string searchTerm = searchTxtBox.Text.ToLower();

            if (searchTerm == "search...")
            {
                wordDataGrid.ItemsSource = allWordTableModels;
                return;
            }

            // Filter the data based on the English word column
            var filteredEntries = allWordTableModels.Where(entry => entry.Word.ToLower().Contains(searchTerm)).ToList();

            // Update the DataGrid with the filtered data
            wordDataGrid.ItemsSource = filteredEntries;
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

        private void SearchTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchFilterTable();
        }

        private void ClearSearch()
        {
            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
        }
        #endregion

        #region Helper
        private void UpdateWordTableModelEntries()
        {
            foreach (var word in WordCollection.Collection[SupportedLanguages.ENGLISH].Keys)
            {
                allWordTableModels.Add(new WordTableModel()
                {
                    Word = word,
                    IsWordSelected = false
                }); ;
            }
        }

        private void UpdateCollectionNamesEntries()
        {
            allCollectionNames = new List<string>();

            foreach (var name in UserCollectionService.Collections.Select(x => x.Name))
            {
                allCollectionNames.Add(name);
            }
        }

        private void CheckCheckBoxesForWords(List<string> wordsToCheck)
        {
            foreach (var item in wordDataGrid.Items)
            {
                if (item is string itemType) // Replace YourItemType with the actual type of items in the DataGrid
                {
                    // Check if the word is in the list
                    if (wordsToCheck.Contains(itemType))
                    {
                        // Set IsSelected to true for the corresponding item
                        wordDataGrid.SelectedItems.Add(item);
                    }
                }
            }
        }
        #endregion

        #region Top Bar
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion
    }
}
