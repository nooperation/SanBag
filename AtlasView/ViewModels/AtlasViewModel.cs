using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using LibSanBag;
using Newtonsoft.Json;
using AtlasView.Commands;
using AtlasView.Models;
using AtlasView.Views;
using CommonUI.ViewModels.ResourceViewModels;
using CommonUI.Views.ResourceViews;

namespace AtlasView.ViewModels
{
    public class AtlasViewModel : INotifyPropertyChanged
    {
        public CommandSearch CommandSearch { get; set; }
        public CommandNextPage CommandNextPage { get; set; }
        public CommandPreviousPage CommandPreviousPage { get; set; }

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

        private string _lastQuery;
        public string LastQuery
        {
            get => _lastQuery;
            set
            {
                _lastQuery = value;
                OnPropertyChanged();
            }
        }

        private List<ExperienceView> _searchResults;
        public List<ExperienceView> SearchResults
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

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;
                ChangePage(value);
                OnPropertyChanged();
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                PageNumbers = new List<int>(Enumerable.Range(1, value));
                OnPropertyChanged();
            }
        }

        private List<int> _pageNumbers;
        public List<int> PageNumbers
        {
            get => _pageNumbers;
            set
            {
                _pageNumbers = value;
                OnPropertyChanged();
            }
        }

        private UserControl _currentAtlasView;
        public UserControl CurrentAtlasView
        {
            get => _currentAtlasView;
            set
            {
                _currentAtlasView = value;
                OnPropertyChanged();
            }
        }

        private ExperienceView _selectedItem;
        public ExperienceView SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                try
                {
                    if (value != null)
                    {
                        OnSelectedItemChanged();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }

                OnPropertyChanged();
            }
        }

        private async void OnSelectedItemChanged()
        {
            if (!(SelectedItem?.DataContext is ExperienceViewModel experienceViewModel))
            {
                return;
            }

            try
            {
                var existingViewModel = CurrentAtlasView?.DataContext as BaseViewModel;
                existingViewModel?.Unload();

                var viewModel = new ManifestResourceViewModel();

                CurrentAtlasView = new LoadingView();

                var downloadManifestResult = await FileRecordInfo.DownloadResourceAsync(
                    experienceViewModel.Experience.Attributes.SceneAssetId,
                    FileRecordInfo.ResourceType.WorldSource,
                    FileRecordInfo.PayloadType.Manifest,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider()
                );

                using (var manifestStream = new MemoryStream(downloadManifestResult.Bytes))
                {
                    CurrentAtlasView = new ManifestResourceView {
                        DataContext = viewModel
                    };
                    viewModel.InitFromStream(manifestStream);
                }
            }
            catch (Exception e)
            {
                CurrentAtlasView = new ErrorView()
                {
                    DataContext = new ErrorViewModel(e)
                };
            }
        }

        public AtlasViewModel()
        {
            CommandSearch = new CommandSearch(this);
            CommandNextPage = new CommandNextPage(this);
            CommandPreviousPage = new CommandPreviousPage(this);

            SearchQuery = "";
            SearchResults = new List<ExperienceView>();
            ExperienceView = "TODO";
            TotalPages = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void Search(string query, int page=1)
        {
            try
            {
                CurrentAtlasView = new LoadingView();

                var perPage = 4;
                var request = WebRequest.Create($"https://atlas.sansar.com/api/experiences?perPage={perPage}&q={query}&page={page}");
                var response = await request.GetResponseAsync();

                string responseJson;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseJson = sr.ReadToEnd();
                }

                var results = JsonConvert.DeserializeObject<AtlasResponse>(responseJson);
                var tempSearchResults = new List<ExperienceView>();
                foreach (var experienceData in results.Data)
                {
                    tempSearchResults.Add(new ExperienceView
                    {
                        DataContext = new ExperienceViewModel(experienceData)
                    });
                }
                SearchResults = tempSearchResults;

                TotalPages = results.Meta.TotalPages;
                LastQuery = query;

                _currentPage = page;
                OnPropertyChanged(nameof(CurrentPage));

                CurrentAtlasView = null;
            }
            catch (Exception e)
            {
                CurrentAtlasView = new ErrorView()
                {
                    DataContext = new ErrorViewModel(e)
                };
            }
        }

        public void ChangePage(int newPage)
        {
            if (newPage > TotalPages || newPage < 1)
            {
                return;
            }

            Search(LastQuery, newPage);
        }

        public void NextPage()
        {
            ChangePage(CurrentPage + 1);
        }

        public void PreviousPage()
        {
            ChangePage(CurrentPage - 1);
        }
    }
}
