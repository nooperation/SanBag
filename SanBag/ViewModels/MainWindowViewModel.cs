using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SanBag.Views;

namespace SanBag.ViewModels
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

            try
            {
                var arguments = Environment.GetCommandLineArgs();
                if (arguments.Length > 1)
                {
                    CurrentView = new BagView();
                    CurrentView.DataContext = new BagViewModel(arguments[1]);
                }
                else
                {
                    CurrentView = new BagView();
                    CurrentView.DataContext = new BagViewModel();
                }
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
