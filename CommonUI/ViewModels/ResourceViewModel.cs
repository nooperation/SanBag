using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonUI.Annotations;
using CommonUI.Views.ResourceViews;
using CommonUI.ViewModels.ResourceViewModels;

namespace CommonUI.ViewModels
{
    public class ResourceViewModel : INotifyPropertyChanged
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

        public ResourceViewModel(Stream resourceStream, LibSanBag.FileRecordInfo.ResourceType resourceType, LibSanBag.FileRecordInfo.PayloadType payloadType, string version, string hash)
        {
            OpenStream(resourceStream, resourceType, payloadType, version, hash);
        }

        public void OpenFile(string resourcePath)
        {
            var fileName = Path.GetFileName(resourcePath);
            var fileNameLower = fileName.ToLower();
            if (fileNameLower.Contains(".bag") && fileNameLower.Contains("userpreferences"))
            {
                CurrentView = new UserPreferencesView();
                CurrentViewModel = new UserPreferencesViewModel();

                CurrentView.DataContext = CurrentViewModel;
                CurrentViewModel.InitFromPath(resourcePath);
            }
            else
            {
                var fileInfo = LibSanBag.FileRecordInfo.Create(fileName);

                using (var resourceStream = File.OpenRead(resourcePath))
                {
                    OpenStream(resourceStream, fileInfo.Resource, fileInfo.Payload, fileInfo.VersionHash, fileInfo.Hash);
                }
            }
        }

        public void OpenStream(Stream resourceStream, LibSanBag.FileRecordInfo.ResourceType resourceType, LibSanBag.FileRecordInfo.PayloadType payloadType, string version, string hash)
        {
            var isRawView = false;

            if (payloadType == LibSanBag.FileRecordInfo.PayloadType.Manifest)
            {
                CurrentView = new ManifestResourceView();
                CurrentViewModel = new ManifestResourceViewModel();
            }
            else if (payloadType == LibSanBag.FileRecordInfo.PayloadType.Debug)
            {
                CurrentView = new RawTextResourceView();
                CurrentViewModel = new RawTextResourceViewModel();
            }
            else if (payloadType == LibSanBag.FileRecordInfo.PayloadType.Payload)
            {
                if (resourceType == LibSanBag.FileRecordInfo.ResourceType.TextureResource)
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
                else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.LicenseResource)
                {
                    CurrentView = new RawTextResourceView();
                    CurrentViewModel = new RawTextResourceViewModel();
                }
                else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.ScriptMetadataResource)
                {
                    CurrentView = new ScriptResourceView();
                    CurrentViewModel = new ScriptResourceViewModel();
                }
                else
                {
                    isRawView = true;
                }
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
                CurrentViewModel.InitFromStream(resourceStream, version, hash);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        public void Unload()
        {
            var previousViewModel = CurrentView?.DataContext as BaseViewModel;
            if (previousViewModel != null)
            {
                previousViewModel.Unload();
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
