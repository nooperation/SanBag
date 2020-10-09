using CommonUI;
using LibSanBag.Providers;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AtlasView.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private Control _currentView;
        public Control CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public MainWindowViewModel()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DependencyChecker.CheckDependencies();

            try
            {
                CurrentView = new Views.AtlasView
                {
                    DataContext = new AtlasViewModel()
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
