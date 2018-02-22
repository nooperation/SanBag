using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LibSanBag;
using LibSanBag.FileResources;
using Newtonsoft.Json;
using SanBag.Commands;
using SanBag.Models;
using SanBag.ViewModels.ResourceViewModels;
using SanBag.Views;
using SanBag.Views.ResourceViews;

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

        private List<Datum> _searchResults;
        public List<Datum> SearchResults
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

        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
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

        private Datum _selectedItem;
        public Datum SelectedItem
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
            if (SelectedItem == null)
            {
                return;
            }

            try
            {
                var viewModel = new ManifestResourceViewModel();

                CurrentAtlasView = new LoadingView();

                var downloadManifestResult = await FileRecordInfo.DownloadResourceAsync(
                    SelectedItem.Attributes.SceneAssetId,
                    FileRecordInfo.ResourceType.WorldSource,
                    FileRecordInfo.PayloadType.Manifest,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider()
                );

                using (var manifestStream = new MemoryStream(downloadManifestResult.Bytes))
                {
                    CurrentAtlasView = new ManifestResourceView();
                    CurrentAtlasView.DataContext = viewModel;
                    viewModel.InitFromStream(manifestStream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to select item: {e.Message}");
            }
        }

        public AtlasViewModel()
        {
            CommandSearch = new CommandSearch(this);

            SearchQuery = "";
            SearchResults = new List<Datum>();
            ExperienceView = "TODO";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Search(string query)
        {
            try
            {
                var perPage = 10;
                var request = WebRequest.Create($"https://atlas.sansar.com/api/experiences?perPage={perPage}&q={query}");
                var response = request.GetResponse();

                var responseJson = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseJson = sr.ReadToEnd();
                }

                var results = JsonConvert.DeserializeObject<AtlasResponse>(responseJson);
                SearchResults = results.Data.ToList();
                TotalPages = results.Meta.TotalPages;
                Page = results.Meta.Page;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to search: {e.Message}");
            }
        }
    }
}
