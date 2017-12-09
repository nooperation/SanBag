using LibSanBag;
using SanBag.Commands;
using SanBag.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SanBag.ViewModels.BagViewModels
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        private CancellationTokenSource ExportCancellationTokenSource { get; set; } = null;

        public CommandCancelExport CommandCancelExport { get; set; }
        public string OutputDirectory { get; set; }
        public string BagPath { get; set; }
        public List<FileRecord> RecordsToExport { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        FileRecord _currentRecord;
        public FileRecord CurrentRecord
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

        public Action<ExportParameters> CustomSaveFunc { get; set; }
        public string FileExtension { get; set; }

        public ExportViewModel()
        {
            CommandCancelExport = new CommandCancelExport(this);
        }

        public void CancelExport()
        {
            if (ExportCancellationTokenSource != null)
            {
                ExportCancellationTokenSource.Cancel();
            }
        }

        private void OnProgressReport(FileRecord record, uint bytesRead)
        {
            MinorProgress = 100.0f * ((float)bytesRead / record.Length);
            TotalRead = bytesRead;
        }

        public bool Export(List<FileRecord> recordsToExport, string bagPath, string outputDirectory, Func<bool> shouldCancel)
        {
            RecordsToExport = recordsToExport;
            var totalExported = 0;
            var exportSuccessful = true;
            FileStream bagStream = null;

            if (!string.IsNullOrWhiteSpace(bagPath))
            {
                bagStream = File.OpenRead(bagPath);
            }

            using (bagStream)
            {
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

                        if (CustomSaveFunc != null)
                        {
                            CustomSaveFunc(new ExportParameters()
                            {
                                FileRecord = record,
                                FileExtension = FileExtension,
                                OutputDirectory = OutputDirectory,
                                BagStream = bagStream,
                                OnProgressReport = OnProgressReport,
                                ShouldCancel = shouldCancel
                            });
                        }
                        else
                        {
                            var outputPath = Path.Combine(outputDirectory, record.Name);
                            using (var out_stream = File.OpenWrite(outputPath))
                            {
                                record.Save(bagStream, out_stream, OnProgressReport, shouldCancel);
                            }
                        }

                        ++totalExported;
                        Progress = 100.0f * (totalExported / (float)RecordsToExport.Count);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export '{record.Name}'\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        exportSuccessful = false;
                        continue;
                    }
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
                    taskWasSuccessful = Export(RecordsToExport, BagPath, OutputDirectory, ShouldCancel);
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
