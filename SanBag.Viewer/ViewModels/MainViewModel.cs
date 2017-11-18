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
using SanBag.Viewer.Annotations;
using SanBag.Viewer.Views;

namespace SanBag.Viewer.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                var arguments = Environment.GetCommandLineArgs();
                if (arguments.Length > 1)
                {
                    OpenFile(arguments[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        public void OpenFile(string resourcePath)
        {
            var fileName = Path.GetFileName(resourcePath);
            var fileInfo = LibSanBag.FileRecordInfo.Create(fileName);

            if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.TextureResource)
            {
                CurrentView = new TextureResourceView()
                {
                    DataContext = new TextureResourceViewModel()
                    {
                        CurrentPath = resourcePath
                    }
                };
            }
            else
            {
                var view = new RawResourceView();
                view.DataContext = new RawResourceViewModel()
                {
                    HexControl = view.HexEdit,
                    CurrentPath = resourcePath,
                };
                CurrentView = view;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
