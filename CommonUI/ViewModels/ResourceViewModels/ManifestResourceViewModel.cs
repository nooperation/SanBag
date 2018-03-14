using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.Providers;
using Microsoft.Win32;
using CommonUI.Commands;
using CommonUI.Models;
using CommonUI.Views;
using CommonUI.Views.ResourceViews;
using ExportView = CommonUI.Views.ExportView;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class ManifestResourceViewModel : BaseViewModel
    {
        public readonly string FilterNone = "None";

        public CommandManifestExportSelected CommandManifestExportSelected { get; set; }

        private List<ManifestResource.ManifestEntry> _manifestList = new List<ManifestResource.ManifestEntry>();

        public List<ManifestResource.ManifestEntry> ManifestList
        {
            get => _manifestList;
            set
            {
                _manifestList = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredManifestList));
                OnPropertyChanged(nameof(Filters));
            }
        }

        public List<ManifestResource.ManifestEntry> FilteredManifestList
        {
            get
            {
                if (CurrentFilter == "None")
                {
                    return _manifestList;
                }
                return _manifestList.Where(n => n.Name.Contains(CurrentFilter)).ToList();
            }
        }

        private string _currentFilter;
        public string CurrentFilter
        {
            get => _currentFilter;
            set
            {
                _currentFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredManifestList));
            }
        }

        public List<string> Filters
        {
            get
            {
                var foundTypes = new HashSet<string>();
                foreach (var manifest in ManifestList)
                {
                    foundTypes.Add(manifest.Name);
                }
                var filters = foundTypes.OrderBy(n => n).ToList();
                filters.Insert(0, FilterNone);

                return filters;
            }
        }

        private UserControl _currentResourceView;
        public UserControl CurrentResourceView
        {
            get => _currentResourceView;
            set
            {
                _currentResourceView = value;
                OnPropertyChanged();
            }
        }

        private ManifestResource.ManifestEntry _selectedRecord;
        public ManifestResource.ManifestEntry SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                _selectedRecord = value;
                try
                {
                    if (value != null)
                    {
                        OnSelectedRecordhanged();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }

                OnPropertyChanged();
            }
        }

        private FileRecordInfo.PayloadType _currentPayloadType;
        public FileRecordInfo.PayloadType CurrentPayloadType
        {
            get => _currentPayloadType;
            set
            {
                _currentPayloadType = value;
                OnSelectedRecordhanged();
                OnPropertyChanged();
            }
        }

        private List<FileRecordInfo.PayloadType> _payloadtypes;
        public List<FileRecordInfo.PayloadType> PayloadTypes
        {
            get => _payloadtypes;
            set
            {
                _payloadtypes = value;
                OnPropertyChanged();
            }
        }

        private bool _isExportingAllVersions;
        public bool IsExportingAllVersions
        {
            get => _isExportingAllVersions;
            set
            {
                _isExportingAllVersions = value;
                OnPropertyChanged();
            }
        }

        private async void OnSelectedRecordhanged()
        {
            if (SelectedRecord == null)
            {
                return;
            }

            try
            {
                var previousViewModel = CurrentResourceView?.DataContext as ResourceViewModel;
                if (previousViewModel != null)
                {
                    previousViewModel.Unload();
                }

                CurrentResourceView = new LoadingView();

                var downloadManifestResult = await FileRecordInfo.DownloadResourceAsync(
                    SelectedRecord.HashString,
                    FileRecordInfo.GetResourceType(SelectedRecord.Name),
                    CurrentPayloadType,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider()
                );

                using (var manifestStream = new MemoryStream(downloadManifestResult.Bytes))
                {
                    var viewModel = new ResourceViewModel(
                        manifestStream,
                        FileRecordInfo.GetResourceType(SelectedRecord.Name),
                        CurrentPayloadType,
                        downloadManifestResult.Version
                    );

                    CurrentResourceView = new ResourceView();
                    CurrentResourceView.DataContext = viewModel;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to select item: {e.Message}");
            }
        }

        public ManifestResourceViewModel()
        {
            Filters.Add(FilterNone);
            CurrentFilter = FilterNone;
            CommandManifestExportSelected = new CommandManifestExportSelected(this);

            PayloadTypes = Enum.GetValues(typeof(FileRecordInfo.PayloadType)).OfType<FileRecordInfo.PayloadType>().ToList();
            CurrentPayloadType = FileRecordInfo.PayloadType.Payload;
            ExportAllVersions = false;
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var resource = ManifestResource.Create(version);
            resource.InitFromStream(resourceStream);
            ManifestList = resource.Entries;
            ManifestList = resource.Entries;
        }

        public void ExportRecords(List<ManifestResource.ManifestEntry> manifestEntriesToExport)
        {
            if (manifestEntriesToExport.Count == 0)
            {
                return;
            }

            var dialog = new SaveFileDialog();
            if (manifestEntriesToExport.Count == 1)
            {
                dialog.FileName = manifestEntriesToExport[0].Name;
            }
            else
            {
                dialog.FileName = "Multiple Files";
            }

            if (dialog.ShowDialog() == true)
            {
                var outputDirectory = Path.GetDirectoryName(dialog.FileName);
                var fileExtension = Path.GetExtension(dialog.FileName);

                var recordsToExport = new List<FileRecord>();
                foreach (var item in manifestEntriesToExport)
                {
                    recordsToExport.Add(new FileRecord()
                    {
                        Name = item.Name,
                        TimestampNs = 0,
                        Offset = 0,
                        Length = 0,
                        Info = new FileRecordInfo()
                        {
                            Hash = item.HashString
                        }
                    });
                }

                var exportViewModel = new ExportViewModel
                {
                    RecordsToExport = recordsToExport,
                    BagPath = null,
                    OutputDirectory = outputDirectory,
                    FileExtension = fileExtension,
                    CustomSaveFunc = CustomFileExport
                };

                var exportDialog = new ExportView
                {
                    DataContext = exportViewModel
                };
                exportDialog.ShowDialog();
            }
        }

        private void ExportAllVersions(ExportParameters exportParameters, List<FileRecordInfo.PayloadType> payloadTypes)
        {
            var assetType = FileRecordInfo.GetResourceType(exportParameters.FileRecord.Name);
            var errorMessages = new StringBuilder();

            var versions = AssetVersions.GetResourceVersions(assetType);
            foreach (var assetVersion in versions)
            {
                foreach (var payloadType in payloadTypes)
                {
                    try
                    {
                        var downloadTask = FileRecordInfo.DownloadResourceAsync(
                            exportParameters.FileRecord.Info?.Hash.ToLower() ?? string.Empty,
                            assetType,
                            payloadType,
                            FileRecordInfo.VariantType.NoVariants,
                            assetVersion,
                            new HttpClientProvider()
                        );
                        var downloadResult = downloadTask.Result;
                        if (downloadResult != null)
                        {
                            var outputPath = Path.Combine(exportParameters.OutputDirectory, downloadResult.Name);

                            using (var outStream = File.OpenWrite(outputPath))
                            {
                                outStream.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessages.AppendLine($"ERROR downloading resource {exportParameters.FileRecord.Info?.Hash ?? string.Empty}.{assetType}.{payloadType}: {ex.Message}");
                        continue;
                    }
                }
            }

            if (errorMessages.Length > 0)
            {
                throw new Exception(errorMessages.ToString());
            }
        }

        private void ExportFirstValidVersion(ExportParameters exportParameters, List<FileRecordInfo.PayloadType> payloadTypes)
        {
            var assetType = FileRecordInfo.GetResourceType(exportParameters.FileRecord.Name);
            var errorMessages = new StringBuilder();

            try
            {
                var downloadTask = FileRecordInfo.DownloadResourceAsync(
                    exportParameters.FileRecord.Info?.Hash.ToLower() ?? string.Empty,
                    assetType,
                    payloadTypes[0],
                    FileRecordInfo.VariantType.NoVariants,
                    new HttpClientProvider()
                );
                var downloadResult = downloadTask.Result;
                if (downloadResult != null)
                {
                    var outputPath = Path.Combine(exportParameters.OutputDirectory, downloadResult.Name);
                    using (var outStream = File.OpenWrite(outputPath))
                    {
                        outStream.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                    }

                    for (int i = 1; i < payloadTypes.Count; i++)
                    {
                        try
                        {
                            downloadTask = FileRecordInfo.DownloadResourceAsync(
                                exportParameters.FileRecord.Info?.Hash.ToLower() ?? string.Empty,
                                assetType,
                                payloadTypes[i],
                                FileRecordInfo.VariantType.NoVariants,
                                new HttpClientProvider()
                            );
                            downloadResult = downloadTask.Result;
                            if (downloadResult != null)
                            {
                                outputPath = Path.Combine(exportParameters.OutputDirectory, downloadResult.Name);
                                using (var outStream = File.OpenWrite(outputPath))
                                {
                                    outStream.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages.AppendLine($"ERROR downloading resource {exportParameters.FileRecord.Info?.Hash ?? string.Empty}.{assetType}.{payloadTypes[i]}: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessages.AppendLine($"ERROR downloading resource {exportParameters.FileRecord.Info?.Hash ?? string.Empty}.{assetType}.{payloadTypes[0]}: {ex.Message}");
            }

            if (errorMessages.Length > 0)
            {
                throw new Exception(errorMessages.ToString());
            }
        }

        protected void CustomFileExport(ExportParameters exportParameters)
        {
            var payloadTypes = new List<FileRecordInfo.PayloadType> {
                FileRecordInfo.PayloadType.Payload,
                FileRecordInfo.PayloadType.Manifest,
            };

            if (IsExportingAllVersions)
            {
                ExportAllVersions(exportParameters, payloadTypes);
            }
            else
            {
                ExportFirstValidVersion(exportParameters, payloadTypes);
            }
        }

        public override void Unload()
        {
            var currentResourceViewModel = CurrentResourceView?.DataContext as ResourceViewModel;
            currentResourceViewModel?.Unload();
        }
    }
}
