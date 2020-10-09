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
using Newtonsoft.Json;
using AtlasView.Commands;
using AtlasView.Models;
using AtlasView.Views;
using CommonUI.ViewModels;
using CommonUI.ViewModels.ResourceViewModels;
using CommonUI.Views;
using CommonUI.Views.ResourceViews;
using LibSanBag.Providers;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

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

                var loadingViewModel = new LoadingViewModel();
                CurrentAtlasView = new LoadingView();
                CurrentAtlasView.DataContext = loadingViewModel;

                var progress = new Progress<ProgressEventArgs>(args => {
                    loadingViewModel.BytesDownloaded = args.BytesDownloaded;
                    loadingViewModel.CurrentResourceIndex = args.CurrentResourceIndex;
                    loadingViewModel.TotalResources = args.TotalResources;
                    loadingViewModel.Status = args.Status;
                    loadingViewModel.TotalBytes = args.TotalBytes;
                    loadingViewModel.DownloadUrl = args.Resource;
                });

                var downloadManifestResult = await FileRecordInfo.DownloadResourceAsync(
                    experienceViewModel.Experience.SceneAssetId,
                    FileRecordInfo.ResourceType.WorldSource,
                    FileRecordInfo.PayloadType.Manifest,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider(),
                    progress
                );

                using (var manifestStream = new MemoryStream(downloadManifestResult.Bytes))
                {
                    CurrentAtlasView = new ManifestResourceView {
                        DataContext = viewModel
                    };
                    viewModel.InitFromStream(manifestStream);
                }
            }
            catch (Exception ex)
            {
                CurrentAtlasView = new ErrorView()
                {
                    DataContext = new ErrorViewModel("Failed to select new item", ex)
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

        private async Task SearchByQuery(string query, int page=1)
        {
            CurrentAtlasView = new LoadingView();

            var loadingViewModel = new LoadingViewModel();
            CurrentAtlasView.DataContext = loadingViewModel;

            var progress = new Progress<ProgressEventArgs>(args => {
                loadingViewModel.BytesDownloaded = args.BytesDownloaded;
                loadingViewModel.CurrentResourceIndex = args.CurrentResourceIndex;
                loadingViewModel.TotalResources = args.TotalResources;
                loadingViewModel.Status = args.Status;
                loadingViewModel.TotalBytes = args.TotalBytes;
                loadingViewModel.DownloadUrl = args.Resource;
            });

            var perPage = 4;
            var client = new HttpClientProvider();
            var responseBytes = await client.GetByteArrayAsync($"https://atlas.sansar.com/proxies/web/atlas-api/v3/experiences?perPage={perPage}&q={query}&page={page}", progress);
            var responseJson = Encoding.ASCII.GetString(responseBytes);

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

            TotalPages = results.Meta.Pages;
            LastQuery = query;

            _currentPage = page;
            OnPropertyChanged(nameof(CurrentPage));

            CurrentAtlasView = null;
        }

        private static async Task<string> DownloadExperienceJsonByUri(string experienceUri, Progress<ProgressEventArgs> progress)
        {
            var match = Regex.Match(experienceUri, @".*/experience/([^/]+/.*)");
            if (!match.Success)
            {
                throw new Exception("Invalid experience URI. Expected to be in the form of sansar://sansar.com/experience/username/experiencename");
            }

            var webUrl = $"https://atlas.sansar.com/experiences/{match.Groups[1].Value}";

            var client = new HttpClientProvider();
            var responseBytes = await client.GetByteArrayAsync(webUrl, progress);
            var responseText = Encoding.ASCII.GetString(responseBytes);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseText);
            var scriptNodes = htmlDocument.DocumentNode.Descendants("script");

            foreach (var item in scriptNodes)
            {
                if (item.InnerText.StartsWith("window.__STATE__="))
                {
                    var scriptSource = item.InnerText.Substring("window.__STATE__=".Length);
                    scriptSource = Regex.Replace(scriptSource, @"new Date\(([^\)]+)\)", "$1");

                    var obj = JObject.Parse(scriptSource);
                    return obj.SelectToken("experience.data").ToString();
                }
            }

            throw new Exception("Failed to parse response.");
        }

        private async Task SearchByUri(string experienceUri)
        {
            CurrentAtlasView = new LoadingView();

            var loadingViewModel = new LoadingViewModel();
            CurrentAtlasView.DataContext = loadingViewModel;

            var progress = new Progress<ProgressEventArgs>(args => {
                loadingViewModel.BytesDownloaded = args.BytesDownloaded;
                loadingViewModel.CurrentResourceIndex = args.CurrentResourceIndex;
                loadingViewModel.TotalResources = args.TotalResources;
                loadingViewModel.Status = args.Status;
                loadingViewModel.TotalBytes = args.TotalBytes;
                loadingViewModel.DownloadUrl = args.Resource;
            });

            var responseJson = await AtlasViewModel.DownloadExperienceJsonByUri(experienceUri, progress);
            var datum = JsonConvert.DeserializeObject<Datum>(responseJson);
            var tempSearchResults = new List<ExperienceView>();
            tempSearchResults.Add(new ExperienceView
            {
                DataContext = new ExperienceViewModel(datum)
            });

            SearchResults = tempSearchResults;

            TotalPages = 1;
            LastQuery = experienceUri;

            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));

            CurrentAtlasView = null;
        }

        public async Task Search(string query, int page=1)
        {
            try
            {
                if (query.ToLower().Contains("/experience/"))
                {
                    await SearchByUri(query);
                }
                else
                {
                    await SearchByQuery(query, page);
                }
            }
            catch (Exception ex)
            {
                CurrentAtlasView = new ErrorView()
                {
                    DataContext = new ErrorViewModel("Failed to search", ex)
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
