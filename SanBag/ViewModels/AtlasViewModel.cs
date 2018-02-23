﻿using System;
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
            CommandNextPage = new CommandNextPage(this);
            CommandPreviousPage = new CommandPreviousPage(this);

            SearchQuery = "";
            SearchResults = new List<Datum>();
            ExperienceView = "TODO";
            TotalPages = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Search(string query, int page=1)
        {
            try
            {
                var perPage = 10;
                var request = WebRequest.Create($"https://atlas.sansar.com/api/experiences?perPage={perPage}&q={query}&page={page}");
                var response = request.GetResponse();

                var responseJson = "";
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseJson = sr.ReadToEnd();
                }

                var results = JsonConvert.DeserializeObject<AtlasResponse>(responseJson);
                SearchResults = results.Data.ToList();
                TotalPages = results.Meta.TotalPages;
                CurrentPage = results.Meta.Page;
                LastQuery = query;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to search: {e.Message}");
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
