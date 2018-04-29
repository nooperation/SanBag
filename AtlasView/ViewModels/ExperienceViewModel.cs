using System.ComponentModel;
using System.Runtime.CompilerServices;
using AtlasView.Models;

namespace AtlasView.ViewModels
{
    class ExperienceViewModel : INotifyPropertyChanged
    {
        public Datum Experience { get; set; }

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get => _thumbnailUrl;
            set
            {
                _thumbnailUrl = value;
                OnPropertyChanged();
            }
        }

        private string _experienceName;
        public string ExperienceName
        {
            get => _experienceName;
            set
            {
                _experienceName = value;
                OnPropertyChanged();
            }
        }

        private string _experienceAuthor;
        public string ExperienceAuthor
        {
            get => _experienceAuthor;
            set
            {
                _experienceAuthor = value;
                OnPropertyChanged();
            }
        }

        public ExperienceViewModel(Datum experienceData)
        {
            Experience = experienceData;
            ThumbnailUrl = Experience.Image.Sizes[0].Url;
            ExperienceAuthor = Experience.PersonaName;
            ExperienceName = Experience.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
