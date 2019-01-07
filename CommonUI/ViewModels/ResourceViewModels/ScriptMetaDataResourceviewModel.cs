using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CommonUI.Commands;
using CommonUI.Views;
using CommonUI.Views.ResourceViews;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.Providers;
using Microsoft.Win32;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class ScriptMetadataResourceViewModel : BaseViewModel, ISavable
    {
        public CommandSaveAs CommandSaveAs { get; set; }

        private UserControl _currentResourceView;
        public UserControl CurrentResourceView
        {
            get => _currentResourceView;
            set
            {
                if (value == _currentResourceView)
                {
                    return;
                }
                _currentResourceView = value;
                OnPropertyChanged();
            }
        }

        private ScriptMetadataResource _resource;
        public ScriptMetadataResource Resource
        {
            get => _resource;
            set
            {
                if (value == _resource)
                {
                    return;
                }
                _resource = value;
                OnPropertyChanged();
            }
        }

        private ScriptMetadataResource.ScriptMetadata _currentScript = new ScriptMetadataResource.ScriptMetadata();
        public ScriptMetadataResource.ScriptMetadata CurrentScript
        {
            get => _currentScript;
            set
            {
                _currentScript = value;
                DumpScriptMetadata();
                OnPropertyChanged();
            }
        }

        public ScriptMetadataResourceViewModel()
        {
            CommandSaveAs = new CommandSaveAs(this);
        }

        private void DumpScriptMetadata()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{CurrentScript.ClassName} ({CurrentScript.DisplayName})");
            if (CurrentScript.Tooltip?.Length > 0)
            {
                sb.AppendLine($"{CurrentScript.Tooltip}");
            }
            sb.AppendLine();

            if(CurrentScript.Properties.Count > 0)
            {
                foreach (var property in CurrentScript.Properties)
                {
                    sb.AppendLine($"    {property.Name} ({property.Type})");
                    if (property.Attributes.Count > 0)
                    {
                        foreach (var attribute in property.Attributes)
                        {
                            var padding = new string(' ', 8 + attribute.Key.Length + 3);
                            var attributeValue = attribute.Value.Replace("\n", "\n" + padding);

                            sb.AppendLine($"        {attribute.Key} = {attributeValue}");
                        }
                    }
                    sb.AppendLine();
                }
            }

            var viewModel = new RawTextResourceViewModel
            {
                CurrentText = sb.ToString()
            };

            CurrentResourceView = new RawTextResourceView
            {
                DataContext = viewModel
            };
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var tempResource = ScriptMetadataResource.Create(version);
            tempResource.InitFromStream(resourceStream);

            Resource = tempResource;
        }

        public async void SaveAs()
        {
            if (Resource == null)
            {
                MessageBox.Show("Attempting to export a null resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var previousView = CurrentResourceView;

                // TODO: Show download dialog?
                var loadingViewModel = new LoadingViewModel();
                var progress = new Progress<ProgressEventArgs>(args => {
                    loadingViewModel.BytesDownloaded = args.BytesDownloaded;
                    loadingViewModel.CurrentResourceIndex = args.CurrentResourceIndex;
                    loadingViewModel.TotalResources = args.TotalResources;
                    loadingViewModel.Status = args.Status;
                    loadingViewModel.TotalBytes = args.TotalBytes;
                    loadingViewModel.DownloadUrl = args.Resource;
                });

                CurrentResourceView = new LoadingView();
                CurrentResourceView.DataContext = loadingViewModel;

                var downloadResult = await FileRecordInfo.DownloadResourceAsync(
                    Hash,
                    FileRecordInfo.ResourceType.ScriptCompiledBytecodeResource,
                    FileRecordInfo.PayloadType.Payload,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider(),
                    progress
                );

                var dialog = new SaveFileDialog();
                dialog.FileName = downloadResult.Name;
                dialog.Filter = "DLL|*.dll";
                if (dialog.ShowDialog() == true)
                {
                    using (var outFile = File.OpenWrite(dialog.FileName))
                    {
                        outFile.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                        MessageBox.Show($"Successfully saved {downloadResult.Bytes.Length} bytes.");
                    }
                }

                CurrentResourceView = previousView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download resource: {ex.Message}");
            }
        }
    }
}
