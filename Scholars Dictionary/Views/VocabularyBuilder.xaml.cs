using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Newtonsoft.Json;
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
        }

        // Buttons
        private void buttonNextWord_Click(object sender, RoutedEventArgs e)
        {
            LoadWord();
        }

        private void LoadWord(string word = "")
        {
            ClearScreen();

            WordDefinition wordDefinition = String.IsNullOrEmpty(word) ? WordService.PickRandomWord() : DefinitionService.GetDefinition(word.Replace(' ', '_'));
            CurrentEnglishWord = wordDefinition.Word;

            TranslateWord(wordDefinition);
            ShowResultsEng(wordDefinition);
            EnglishSpeaker.Visibility = Visibility.Visible;
        }

        // Word Definition
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
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Name.ToString().Contains("Russian"))
                {
                    IsRussianChecked = false;
                }
                else if (checkBox.Name.ToString().Contains("Spanish"))
                {
                    IsSpanishChecked = false;
                }
            }
        }

        // Helper Methods
        private void ClearScreen(bool clearEnglish = false)
        {
            if (clearEnglish)
            {
                englishRTB.Document.Blocks.Clear();
            }
            spanishRTB.Document.Blocks.Clear();
            russianRTB.Document.Blocks.Clear();
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
    }
}
