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

        public ResourceViewModel(Stream resourceStream, LibSanBag.FileRecordInfo.ResourceType resourceType, LibSanBag.FileRecordInfo.PayloadType payloadType, string version)
        {
            OpenStream(resourceStream, resourceType, payloadType, version);
        }

        public void OpenFile(string resourcePath)
        {
            var fileName = Path.GetFileName(resourcePath);
            var fileInfo = LibSanBag.FileRecordInfo.Create(fileName);

            using (var resourceStream = File.OpenRead(resourcePath))
            {
                OpenStream(resourceStream, fileInfo.Resource, fileInfo.Payload, fileInfo.VersionHash);
            }
        }

        public void OpenStream(Stream resourceStream, LibSanBag.FileRecordInfo.ResourceType resourceType, LibSanBag.FileRecordInfo.PayloadType payloadType, string version)
        {
            var isRawView = false;

            if (payloadType == LibSanBag.FileRecordInfo.PayloadType.Manifest)
            {
                CurrentView = new ManifestResourceView();
                CurrentViewModel = new ManifestResourceViewModel();
            }
            else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.TextureResource)
            {
                CurrentView = new TextureResourceView();
                CurrentViewModel = new TextureResourceViewModel();
            }
            else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.SoundResource)
            {
                CurrentView = new SoundResourceView();
                CurrentViewModel = new SoundResourceViewModel();
            }
            else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.ScriptSourceTextResource ||
                     resourceType == LibSanBag.FileRecordInfo.ResourceType.LuaScriptResource)
            {
                CurrentView = new ScriptSourceTextView();
                CurrentViewModel = new ScriptSourceTextViewModel();
            }
            else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.GeometryResourceResource)
            {
                CurrentView = new GeometryResourceView();
                CurrentViewModel = new GeometryResourceViewModel();
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
                CurrentViewModel.InitFromStream(resourceStream, version);
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
