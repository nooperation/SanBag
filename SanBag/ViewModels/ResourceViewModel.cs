using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SanBag.Annotations;
using SanBag.Views.Standalone;
using SanBag.ViewModels.Standalone;

namespace SanBag.ViewModels
{
    class ResourceViewModel : INotifyPropertyChanged
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

        public ResourceViewModel(string fileToOpen)
        {
            OpenFile(fileToOpen);
        }

        public void OpenFile(string resourcePath)
        {
            var fileName = Path.GetFileName(resourcePath);
            var fileInfo = LibSanBag.FileRecordInfo.Create(fileName);

            if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.TextureResource)
            {
                CurrentView = new SanBag.Views.Standalone.TextureResourceView()
                {
                    DataContext = new SanBag.ViewModels.Standalone.TextureResourceViewModel()
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
