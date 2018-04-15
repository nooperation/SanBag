using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CommonUI.ViewModels
{
    public class ErrorViewModel : INotifyPropertyChanged
    {
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ErrorViewModel()
        {
            ErrorMessage = "Error";
        }

        public ErrorViewModel(string errorMessage, Exception ex)
        {
            ErrorMessage = errorMessage + ": " + ex.Message;
        }

        public ErrorViewModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
