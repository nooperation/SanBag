using LibSanBag;
using SanBag.Commands;
using SanBag.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LibSanBag.Providers;
using static LibSanBag.FileResources.ManifestResource;

namespace SanBag.ViewModels.BagViewModels
{
    public class ExportManifestViewModel : INotifyPropertyChanged
    {
        private CancellationTokenSource ExportCancellationTokenSource { get; set; } = null;

        public CommandManifestCancelExport CommandManifestCancelExport { get; set; }
        public string OutputDirectory { get; set; }
        public List<ManifestEntry> RecordsToExport { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ManifestEntry _currentRecord;
        public ManifestEntry CurrentRecord
        {
            get => _currentRecord;
            set
            {
                _currentRecord = value;
                OnPropertyChanged();
            }
        }

        private float _progress;
        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        private float _minorProgress;
        public float MinorProgress
        {
            get => _minorProgress;
            set
            {
                _minorProgress = value;
                OnPropertyChanged();
            }
        }

        public uint _totalRead;
        public uint TotalRead
        {
            get => _totalRead;
            set
            {
                _totalRead = value;
                OnPropertyChanged();
            }
        }

        private bool _isRunning = true;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public ExportManifestViewModel()
        {
            CommandManifestCancelExport = new CommandManifestCancelExport(this);
        }

        public void CancelExport()
        {
            if (ExportCancellationTokenSource != null)
            {
                ExportCancellationTokenSource.Cancel();
            }
        }

        public bool Export(List<ManifestEntry> recordsToExport, string outputDirectory, Func<bool> shouldCancel)
        {
            RecordsToExport = recordsToExport;
            var totalExported = 0;
            var exportSuccessful = true;

            foreach (var record in recordsToExport)
            {
                if (shouldCancel != null && shouldCancel())
                {
                    exportSuccessful = false;
                    break;
                }

                try
                {
                    CurrentRecord = record;
                    var payloadTypes = new List<FileRecordInfo.PayloadType>{ FileRecordInfo.PayloadType.Payload, FileRecordInfo.PayloadType.Manifest };
                    var assetType = FileRecordInfo.GetResourceType(record.Name);
                    var assetVersions = AssetVersions.GetResourceVersions(assetType);

                    foreach (var payloadType in payloadTypes)
                    {
                        FileRecordInfo.DownloadResults downloadResult;
                        try
                        {
                            downloadResult = FileRecordInfo.DownloadResourceAsync(record.HashString.ToLower(), assetType, payloadType, FileRecordInfo.VariantType.NoVariants, new HttpClientProvider()).Result;
                            if (downloadResult != null)
                            {
                                var outputPath = Path.Combine(outputDirectory, downloadResult.Name);

                                using (var out_stream = File.OpenWrite(outputPath))
                                {
                                    out_stream.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                                }

                                ++totalExported;
                                Progress = 100.0f * (totalExported / (float)(RecordsToExport.Count * payloadTypes.Count));
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export '{record.Name}'\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    exportSuccessful = false;
                    continue;
                }
            }

            return exportSuccessful;
        }

        private bool ShouldCancel()
        {
            return ExportCancellationTokenSource.IsCancellationRequested;
        }

        public async Task StartAsync()
        {
            IsRunning = true;
            ExportCancellationTokenSource = new CancellationTokenSource();
            var taskWasSuccessful = false;

            await Task.Run(() =>
            {
                try
                {
                    taskWasSuccessful = Export(RecordsToExport, OutputDirectory, ShouldCancel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export: {ex.Message}");
                }
            }, ExportCancellationTokenSource.Token);

            if (taskWasSuccessful)
            {
                foreach (var item in Application.Current.Windows)
                {
                    var window = item as Window;
                    if (window != null && window.DataContext == this)
                    {
                        window.Close();
                        break;
                    }
                }
                MessageBox.Show($"Successfully exported {RecordsToExport.Count} record(s) to {OutputDirectory}", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            IsRunning = false;
        }
    }
}
