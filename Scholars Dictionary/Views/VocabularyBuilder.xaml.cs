using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
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

        public VocabularyBuilder()
        {
            InitializeComponent();

            EnglishSpeaker.Visibility = Visibility.Hidden;
            RussianSpeaker.Visibility = Visibility.Hidden;
            SpanishSpeaker.Visibility = Visibility.Hidden;

            searchTxtBox.Text = "Search...";
            searchTxtBox.Foreground = Brushes.Gray;
        }

        // Buttons
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
            ShowResultsEng(wordDefinition);
            EnglishSpeaker.Visibility = Visibility.Visible;
        }

        // Word Definition
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
        private void ShowResultsEng(WordDefinition wordDefinition)
        {
            // Create a new FlowDocument
            FlowDocument flowDocument = new FlowDocument();

            // Create a Paragraph
            Paragraph paragraph = new Paragraph();
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");

            int index = 0;
            foreach (var type in wordDefinition.Types)
            {
                // Add Runs with different formatting
                if (index != 0)
                {
                    paragraph.Inlines.Add(new LineBreak());
                }
                Run normalRun = new Run(wordDefinition.Word) { FontSize = 22 };
                paragraph.Inlines.Add(normalRun);

                Run superscriptRun = new Run($" ({EnumHelper.WordTypeString(type)})") { FontSize = 12 };
                superscriptRun.BaselineAlignment = BaselineAlignment.Superscript;
                paragraph.Inlines.Add(superscriptRun);


                FormatText(paragraph, wordDefinition.Definitions[index]);

                index++;
            }

            paragraph.Inlines.Add(new LineBreak());
            FormatRelatedWords(paragraph, wordDefinition.RelatedWords);

            flowDocument.Blocks.Add(paragraph);

            englishRTB.Document = flowDocument;
        }

        private void FormatText(Paragraph paragraph, string inputString)
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
                            paragraph.Inlines.Add(new Run($"\t{line}") { FontStyle = FontStyles.Italic, FontSize = 15, FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight") });
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
                            paragraph.Inlines.Add(new Run($"\t{bullet} {line}") { FontWeight = FontWeights.Bold, FontSize = 15, FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight") });
                            index++;
                        }
                    }
                }
            }
        }

        private void FormatRelatedWords(Paragraph paragraph, List<string> relatedWords)
        {
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");
            int index = 0;
            foreach (string word in relatedWords)
            {
                if(index > 0)
                {
                    paragraph.Inlines.Add(", ");
                }
                AddClickableLink(paragraph, word, () => { LoadWord(word); });
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

        // Translating
        private async void TranslateWord(WordDefinition wordDefinition)
        {
            if (IsRussianChecked)
            {
                var response = await AzureTranslateAPI.TranslateText(wordDefinition.Word, SupportedLanguages.ENGLISH.GetStringValue(), SupportedLanguages.RUSSIAN.GetStringValue());
                var translatedText = (JArray.Parse(response))[0]["translations"][0]["text"].ToString();
                ShowResultTranslate(translatedText, SupportedLanguages.RUSSIAN);

                CurrentRussianWord = translatedText;
                RussianSpeaker.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentRussianWord = "";
                RussianSpeaker.Visibility = Visibility.Hidden;
            }
            if (IsSpanishChecked)
            {
                var response = await AzureTranslateAPI.TranslateText(wordDefinition.Word, SupportedLanguages.ENGLISH.GetStringValue(), SupportedLanguages.SPANISH.GetStringValue());
                var translatedText = (JArray.Parse(response))[0]["translations"][0]["text"].ToString();
                ShowResultTranslate(translatedText, SupportedLanguages.SPANISH);

                CurrentSpanishWord = translatedText;
                SpanishSpeaker.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentSpanishWord = "";
                SpanishSpeaker.Visibility = Visibility.Hidden;
            }
        }

        private void ShowResultTranslate(string translatedText, SupportedLanguages language)
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.TextAlignment = TextAlignment.Center;
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Bahnschrift SemiLight");
            Run run = new Run(translatedText) { FontSize = 22 };
            paragraph.Inlines.Add(run);
            document.Blocks.Add(paragraph);

            switch (language)
            {
                case SupportedLanguages.RUSSIAN:
                    russianRTB.Document = document;
                    break;
                case SupportedLanguages.SPANISH:
                    spanishRTB.Document = document;
                    break;
            }
        }

        // Voice
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

        // Language Selection
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

        // Helper Methods
        private void ClearScreen(bool clearEnglish = false, bool clearSearch = false)
        {
            if (clearEnglish)
            {
                englishRTB.Document.Blocks.Clear();
            }
            spanishRTB.Document.Blocks.Clear();
            russianRTB.Document.Blocks.Clear();

            if (clearSearch)
            {
                ClearSearch();
            }
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

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        // Search
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
    }
}
