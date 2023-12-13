﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        public VocabularyBuilder()
        {
            InitializeComponent();
        }

        // Buttons
        private void buttonNextWord_Click(object sender, RoutedEventArgs e)
        {
            LoadNextWord();
        }

        private void LoadNextWord()
        {
            string newWord = RandomWordPicker.PickRandomWord();

            WordDefinition wordDefinition = new WordDefinition();
            while (wordDefinition.Definitions.Count == 0)
            {
                wordDefinition = DefinitionService.GetDefinition(newWord);
            }

            ShowResults(wordDefinition);
        }

        private void LoadWord(string word)
        {
            WordDefinition wordDefinition = DefinitionService.GetDefinition(word);
            ShowResults(wordDefinition);
        }

        private void ShowResults(WordDefinition wordDefinition)
        {
            // Create a new FlowDocument
            FlowDocument flowDocument = new FlowDocument();

            // Create a Paragraph
            Paragraph paragraph = new Paragraph();

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

            // Add links for related words
            paragraph.Inlines.Add(new LineBreak());
            FormatRelatedWords(paragraph, wordDefinition.RelatedWords);

            flowDocument.Blocks.Add(paragraph);

            englishRTB.Document = flowDocument;
        }

        private void FormatText(Paragraph paragraph, string inputString)
        {
            // Split the input string by bullet points
            string[] bulletPoints = inputString.Split(new[] { " • " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string bulletPoint in bulletPoints)
            {
                // Split each bullet point by semicolons to separate the parts
                string[] parts = bulletPoint.Split(';');

                int maxBoldCharactersPerLine = 88;
                int maxItalicCharactersPerLine = 100;

                foreach (string part in parts)
                {
                    // Check if the part contains quotes
                    if (part.Contains("\""))
                    {
                        // If it contains quotes, make it italic
                        string[] lines = SplitTextByLineLength(part, maxItalicCharactersPerLine);
                        int index = 0;
                        foreach (string line in lines)
                        {
                            if ((line == lines[lines.Length - 1]) && !string.IsNullOrEmpty(line) && index > 0 && char.IsLetter(lines[index-1][lines[index - 1].Length - 1]))
                            {
                                paragraph.Inlines.Add("-");
                            }
                            paragraph.Inlines.Add(new LineBreak());
                            paragraph.Inlines.Add(new Run($"\t{line}") { FontStyle = FontStyles.Italic, FontSize = 15 });
                            index++;
                        }
                    }
                    else
                    {
                        // If it doesn't contain quotes, make it bold
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
                            paragraph.Inlines.Add(new Run($"\t{bullet} {line}") { FontWeight = FontWeights.Bold, FontSize = 15 });
                            index++;
                        }
                    }
                }
            }
        }

        private void FormatRelatedWords(Paragraph paragraph, List<string> relatedWords)
        {
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
            // Create a new Hyperlink
            Hyperlink hyperlink = new Hyperlink(new Run(linkText));

            // Set the Click event handler
            hyperlink.Click += (sender, e) => onClickAction.Invoke();

            // Create a new Paragraph and add the Hyperlink to it
            paragraph.Inlines.Add(hyperlink);
        }

        // Helper method to split text into lines based on maximum characters per line
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
