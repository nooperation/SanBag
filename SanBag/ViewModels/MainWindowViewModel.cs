using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonUI.ViewModels;
using CommonUI.Views;
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
                var arguments = Environment.GetCommandLineArgs();
                if (arguments.Length > 1)
                {
                    var firstArgument = arguments[1].Trim().ToLower();

                    var extension = Path.GetExtension(firstArgument);
                    if (extension == ".bag" && firstArgument.IndexOf("userpreferences", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        CurrentView = new BagView();
                        CurrentView.DataContext = new BagViewModel(arguments[1]);
                    }
                    else
                    {
                        CurrentView = new ResourceView();
                        CurrentView.DataContext = new ResourceViewModel(arguments[1]);
                    }
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
