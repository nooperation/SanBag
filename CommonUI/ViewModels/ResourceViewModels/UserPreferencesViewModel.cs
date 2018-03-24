using System.IO;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class UserPreferencesViewModel : BaseViewModel
    {
        private string _currentText;
        public string CurrentText
        {
            get => _currentText;
            set
            {
                if (value == _currentText)
                {
                    return;
                }
                _currentText = value;
                OnPropertyChanged();
            }
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            using (var sr = new StreamReader(resourceStream))
            {
                CurrentText = "TODO";
            }
        }
    }
}
