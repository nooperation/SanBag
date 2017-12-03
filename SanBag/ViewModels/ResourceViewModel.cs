using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SanBag.Annotations;
using SanBag.Views.ResourceViews;
using SanBag.ViewModels.ResourceViewModels;

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
                CurrentView = new SanBag.Views.ResourceViews.TextureResourceView()
                {
                    DataContext = new SanBag.ViewModels.ResourceViewModels.TextureResourceViewModel()
                    {
                        CurrentPath = resourcePath
                    }
                };
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.SoundResource)
            {
                CurrentView = new SanBag.Views.ResourceViews.SoundResourceView()
                {
                    DataContext = new SanBag.ViewModels.ResourceViewModels.SoundResourceViewModel()
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
