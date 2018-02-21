using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SanBag.Commands;

namespace SanBag.ViewModels
{
    class AtlasViewModel : INotifyPropertyChanged
    {
        public CommandSearch CommandSearch { get; set; }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        private List<string> _searchResults;
        public List<string> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        private object _experienceView;
        public object ExperienceView
        {
            get => _experienceView;
            set
            {
                _experienceView = value;
                OnPropertyChanged();
            }
        }

        public AtlasViewModel()
        {
            CommandSearch = new CommandSearch(this);

            SearchQuery = "...";
            SearchResults = new List<string>()
            {
                "Foo",
                "Bar",
                "Baz"
            };
            ExperienceView = "Butts";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Search(string query)
        {
            MessageBox.Show($"Searched for '{query}'");
        }
    }
}
