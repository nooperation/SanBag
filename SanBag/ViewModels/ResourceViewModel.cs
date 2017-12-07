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
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

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
                CurrentView = new SanBag.Views.ResourceViews.TextureResourceView();
                CurrentViewModel = new SanBag.ViewModels.ResourceViewModels.TextureResourceViewModel();
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.SoundResource)
            {
                CurrentView = new SanBag.Views.ResourceViews.SoundResourceView();
                CurrentViewModel = new SanBag.ViewModels.ResourceViewModels.SoundResourceViewModel();
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.ScriptSourceTextResource ||
                     fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.LuaScriptResource)
            {
                CurrentView = new SanBag.Views.ResourceViews.ScriptSourceTextView();
                CurrentViewModel = new SanBag.ViewModels.ResourceViewModels.ScriptSourceTextViewModel();
            }
            else
            {
                var view = new RawResourceView();
                var model = new RawResourceViewModel {
                    HexControl = view.HexEdit
                };
                CurrentView = view;
                CurrentViewModel = model;
            }

            CurrentView.DataContext = CurrentViewModel;
            CurrentViewModel.InitFromPath(resourcePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
