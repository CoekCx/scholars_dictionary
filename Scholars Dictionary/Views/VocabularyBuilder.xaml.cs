using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Scholars_Dictionary.Enums;
using Scholars_Dictionary.Models;
using Scholars_Dictionary.Services;

namespace Scholars_Dictionary.Views
{
    /// <summary>
    /// Interaction logic for VocabularyBuilder.xaml
    /// </summary>
    public partial class VocabularyBuilder : Window
    {
        #region Properties
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

        public string CurrentEnglishWord { get; set; } = "";
        public string CurrentRussianWord { get; set; } = "";
        public string CurrentSpanishWord { get; set; } = "";
        #endregion

        public VocabularyBuilder()
        {
            InitializeComponent();

            EnglishSpeaker.Visibility = Visibility.Hidden;
            RussianSpeaker.Visibility = Visibility.Hidden;
            SpanishSpeaker.Visibility = Visibility.Hidden;

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
        }

        #region Buttons
        private void buttonGenerateWord_Click(object sender, RoutedEventArgs e)
        {
            LoadWord();
        }

        private void LoadWord(string word = "", bool clearSearch = false)
        {
            ClearScreen(clearSearch: clearSearch);

            WordDefinition wordDefinition = String.IsNullOrEmpty(word) ? WordService.PickRandomWord() : DefinitionService.GetDefinition(word.Replace(' ', '_'));
            CurrentEnglishWord = wordDefinition.Word;

            if (!wordDefinition.IsDefined())
            {
                ShowUndefinedResult(wordDefinition);
                EnglishSpeaker.Visibility = Visibility.Hidden;
                RussianSpeaker.Visibility = Visibility.Hidden;
                SpanishSpeaker.Visibility = Visibility.Hidden;
                return;
            }

            TranslateWord(wordDefinition);
            var doc = FormatWordDefinition(wordDefinition);
            englishRTB.Document = doc;
            EnglishSpeaker.Visibility = Visibility.Visible;
        }
        #endregion

        #region Definition
        private void ShowUndefinedResult(WordDefinition wordDefinition)
        {
            FlowDocument flowDocument = new FlowDocument();

            Paragraph paragraph = new Paragraph();
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");
            paragraph.FontSize = 22;

            Run run1 = new Run("No definition found for the term ");
            Run run2 = new Run($"\"{wordDefinition.Word}\"") { FontStyle = FontStyles.Italic };
            paragraph.Inlines.Add(run1);
            paragraph.Inlines.Add(run2);

            flowDocument.Blocks.Add(paragraph);
            englishRTB.Document = flowDocument;
        }

        private FlowDocument FormatWordDefinition(WordDefinition wordDefinition, SupportedLanguages language = SupportedLanguages.ENGLISH)
        {
            double wordSize = language.Equals(SupportedLanguages.ENGLISH) ? 22 : 20;
            double typeSize = language.Equals(SupportedLanguages.ENGLISH) ? 12 : 11;
            double definitionSize = language.Equals(SupportedLanguages.ENGLISH) ? 15 : 13.5;
            FlowDocument flowDocument = new FlowDocument();

            Paragraph paragraph = new Paragraph();
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");

            int index = 0;
            foreach (var type in wordDefinition.Types)
            {
                if (index != 0)
                {
                    paragraph.Inlines.Add(new LineBreak());
                }
                Run normalRun = new Run(wordDefinition.Word) { FontSize = wordSize };
                paragraph.Inlines.Add(normalRun);

                Run superscriptRun = new Run($" ({EnumHelper.WordTypeString(type)})") { FontSize = typeSize };
                superscriptRun.BaselineAlignment = BaselineAlignment.Superscript;
                paragraph.Inlines.Add(superscriptRun);


                FormatDefinition(paragraph, wordDefinition.Definitions[index], definitionSize);

                index++;
            }

            paragraph.Inlines.Add(new LineBreak());
            FormatRelatedWords(paragraph, wordDefinition.RelatedWords, language);

            flowDocument.Blocks.Add(paragraph);

            return flowDocument;
        }

        private void FormatDefinition(Paragraph paragraph, string inputString, double defSize)
        {
            string[] bulletPoints = inputString.Split(new[] { " • " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string bulletPoint in bulletPoints)
            {
                string[] parts = bulletPoint.Split(';');

                int maxBoldCharactersPerLine = 88;
                int maxItalicCharactersPerLine = 100;

                foreach (string part in parts)
                {
                    if (part.Contains("\""))
                    {
                        string[] lines = SplitTextByLineLength(part, maxItalicCharactersPerLine);
                        int index = 0;
                        foreach (string line in lines)
                        {
                            if ((line == lines[lines.Length - 1]) && !string.IsNullOrEmpty(line) && index > 0 && char.IsLetter(lines[index-1][lines[index - 1].Length - 1]))
                            {
                                paragraph.Inlines.Add("-");
                            }
                            paragraph.Inlines.Add(new LineBreak());
                            paragraph.Inlines.Add(new Run($"\t{line}") { FontStyle = FontStyles.Italic, FontSize = defSize, FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight") });
                            index++;
                        }
                    }
                    else
                    {
                        string[] lines = SplitTextByLineLength(part, maxBoldCharactersPerLine);
                        int index = 0;
                        foreach (string line in lines)
                        {
                            if ((line == lines[lines.Length - 1]) && !string.IsNullOrEmpty(line) && index > 0 && char.IsLetter(lines[index - 1][lines[index - 1].Length - 1]))
                            {
                                paragraph.Inlines.Add("-");
                            }
                            paragraph.Inlines.Add(new LineBreak());
                            string bullet = line == lines[0] ? "•" : " ";
                            paragraph.Inlines.Add(new Run($"\t{bullet} {line}") { FontWeight = FontWeights.Bold, FontSize = defSize, FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight") });
                            index++;
                        }
                    }
                }
            }
        }

        private void FormatRelatedWords(Paragraph paragraph, List<string> relatedWords, SupportedLanguages language)
        {
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");
            int index = 0;
            foreach (string word in relatedWords)
            {
                if(index > 0)
                {
                    paragraph.Inlines.Add(", ");
                }
                AddClickableLink(paragraph, word, async () =>  {
                    if (language.Equals(SupportedLanguages.ENGLISH))
                    {
                        LoadWord(word);
                    }
                    else
                    {
                        var translatedWord = await AzureTranslateAPI.TranslateText(word, language.GetStringValue(), SupportedLanguages.ENGLISH.GetStringValue());
                        LoadWord(translatedWord);
                    }
                });
                index++;
            }
        }

        private void AddClickableLink(Paragraph paragraph, string linkText, Action onClickAction)
        {
            Hyperlink hyperlink = new Hyperlink(new Run(linkText));
            hyperlink.Click += (sender, e) => onClickAction.Invoke();
            paragraph.Inlines.Add(hyperlink);
        }

        private string[] SplitTextByLineLength(string text, int maxCharacters)
        {
            List<string> lines = new List<string>();
            int index = 0;

            while (index < text.Length)
            {
                int length = Math.Min(maxCharacters, text.Length - index);
                string line = text.Substring(index, length);
                lines.Add(line);
                index += length;
            }

            return lines.ToArray();
        }
        #endregion

        #region Translating
        private async void TranslateWord(WordDefinition wordDefinition)
        {
            if (IsRussianChecked)
            {
                if (russianRTB.Document.Blocks.Count == 0 && wordDefinition.TryDefineSelf())
                {
                    var translatedWordDefinition = await AzureTranslateAPI.TranslateWordDefinition(wordDefinition, SupportedLanguages.ENGLISH.GetStringValue(), SupportedLanguages.RUSSIAN.GetStringValue());
                    ShowResultTranslate(translatedWordDefinition, SupportedLanguages.RUSSIAN);

                    CurrentRussianWord = translatedWordDefinition.Word;
                    RussianSpeaker.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CurrentRussianWord = "";
                RussianSpeaker.Visibility = Visibility.Hidden;
            }
            if (IsSpanishChecked)
            {
                if (spanishRTB.Document.Blocks.Count == 0 && wordDefinition.TryDefineSelf())
                {
                    var translatedWordDefinition = await AzureTranslateAPI.TranslateWordDefinition(wordDefinition, SupportedLanguages.ENGLISH.GetStringValue(), SupportedLanguages.SPANISH.GetStringValue());
                    ShowResultTranslate(translatedWordDefinition, SupportedLanguages.SPANISH);

                    CurrentSpanishWord = translatedWordDefinition.Word;
                    SpanishSpeaker.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CurrentSpanishWord = "";
                SpanishSpeaker.Visibility = Visibility.Hidden;
            }
        }

        private void ShowResultTranslate(WordDefinition translatedWordDefinition, SupportedLanguages language)
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.TextAlignment = TextAlignment.Center;
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");
            Run run = new Run(translatedWordDefinition.Word) { FontSize = 22 };
            paragraph.Inlines.Add(run);
            document.Blocks.Add(paragraph);

            var doc = FormatWordDefinition(translatedWordDefinition, language);

            switch (language)
            {
                case SupportedLanguages.RUSSIAN:
                    russianRTB.Document = doc;
                    break;
                case SupportedLanguages.SPANISH:
                    spanishRTB.Document = doc;
                    break;
            }

        }
        #endregion

        #region Voice
        private void Speak(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img)
            {
                switch (img.Name)
                {
                    case "EnglishSpeaker":
                        AzureTTSAPI.TextToSpeech(CurrentEnglishWord, SupportedLanguages.ENGLISH);
                        break;
                    case "RussianSpeaker":
                        AzureTTSAPI.TextToSpeech(CurrentRussianWord, SupportedLanguages.RUSSIAN);
                        break;
                    case "SpanishSpeaker":
                        AzureTTSAPI.TextToSpeech(CurrentSpanishWord, SupportedLanguages.SPANISH);
                        break;
                }
            }
        }
        #endregion

        #region Language Selection
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Name.ToString().Contains("Russian"))
                {
                    IsRussianChecked = true;
                }
                else if (checkBox.Name.ToString().Contains("Spanish"))
                {
                    IsSpanishChecked = true;
                }

                if (!String.IsNullOrEmpty(CurrentEnglishWord))
                {
                    TranslateWord(new WordDefinition() { Word = CurrentEnglishWord });
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Name.ToString().Contains("Russian"))
                {
                    IsRussianChecked = false;
                    russianRTB.Document.Blocks.Clear();
                    RussianSpeaker.Visibility = Visibility.Hidden;
                }
                else if (checkBox.Name.ToString().Contains("Spanish"))
                {
                    IsSpanishChecked = false;
                    spanishRTB.Document.Blocks.Clear();
                    SpanishSpeaker.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        #region Helper Methods
        private void ClearScreen(bool clearEnglish = false, bool clearSearch = false)
        {
            if (clearEnglish)
            {
                englishRTB.Document.Blocks.Clear();
            }
            russianRTB.Document.Blocks.Clear();
            spanishRTB.Document.Blocks.Clear();

            if (clearSearch)
            {
                ClearSearch();
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
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion

        #region Search
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

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrEmpty(searchTxtBox.Text) && !String.Equals(searchTxtBox.Text, "Search..."))
            {
                // Enter key pressed
                Search();
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

        private void LookUp_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(searchTxtBox.Text) && !String.Equals(searchTxtBox.Text, "Search..."))
            {
                Search();
            }
        }

        private void Search()
        {
            string searchTerm = searchTxtBox.Text;

            LoadWord(searchTerm, true);

            SearchBtn.Focus();
        }

        private void ClearSearch()
        {
            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
        }
        #endregion

        #region Favorite
        private void buttonFavorite_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
