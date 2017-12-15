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

            if (fileInfo?.Payload == LibSanBag.FileRecordInfo.PayloadType.Manifest)
            {
                CurrentView = new ManifestResourceView();
                CurrentViewModel = new ManifestResourceViewModel();
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.TextureResource)
            {
                CurrentView = new TextureResourceView();
                CurrentViewModel = new TextureResourceViewModel();
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.SoundResource)
            {
                CurrentView = new SoundResourceView();
                CurrentViewModel = new SoundResourceViewModel();
            }
            else if (fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.ScriptSourceTextResource ||
                     fileInfo?.Resource == LibSanBag.FileRecordInfo.ResourceType.LuaScriptResource)
            {
                CurrentView = new ScriptSourceTextView();
                CurrentViewModel = new ScriptSourceTextViewModel();
            }
            else
            {
                isRawView = true;
            }

            if (isRawView)
            {
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
            try
            {
                CurrentView.DataContext = CurrentViewModel;
                CurrentViewModel.InitFromPath(resourcePath);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
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
