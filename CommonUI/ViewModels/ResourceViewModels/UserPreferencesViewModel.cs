using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                var json = LibSanBag.ResourceUtils.LibUserPreferences.Read(sr.BaseStream, null, null);
                var root = JToken.Parse(json);
                CurrentText = root.ToString(Formatting.Indented);
            }
        }
    }
}
