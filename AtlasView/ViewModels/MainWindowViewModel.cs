using System;
using System.ComponentModel;
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

            if(!LibSanBag.ResourceUtils.Unpacker.IsAvailable)
            {
                MessageBox.Show(
                    "This program requires additional dependencies to run. Please obtain one of the following DLLs and place it in the directory containing " + System.AppDomain.CurrentDomain.FriendlyName + ":\n" +
                    "  oo2core_6_win64.dll\n" +
                    "  oo2core_7_win64.dll",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
            }

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
