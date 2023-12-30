using System.ComponentModel;

namespace Scholars_Dictionary.Models
{
    public class WordTableModel : INotifyPropertyChanged
    {
        private string word;
        public string Word
        {
            get { return word; }
            set
            {
                if (word != value)
                {
                    word = value;
                    OnPropertyChanged(nameof(Word));
                }
            }
        }

        private bool isWordSelected;
        public bool IsWordSelected
        {
            get { return isWordSelected; }
            set
            {
                if (isWordSelected != value)
                {
                    isWordSelected = value;
                    OnPropertyChanged(nameof(IsWordSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
