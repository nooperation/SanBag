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
            var isRawView = false;

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
                isRawView = true;
            }

            if (isRawView == false)
            {
                try
                {
                    CurrentView.DataContext = CurrentViewModel;
                    CurrentViewModel.InitFromPath(resourcePath);
                    return;
                }
                catch (Exception ex)
                {
                    isRawView = true;
                    MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }

            try
            {
                var view = new RawResourceView();
                var model = new RawResourceViewModel
                {
                    HexControl = view.HexEdit
                };
                CurrentView = view;
                CurrentViewModel = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load raw view: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
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
