using LibSanBag;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SanBag.Views.BagViews;
using SanBag.Views.ResourceViews;
using ExportView = SanBag.Views.ExportView;

namespace SanBag.ViewModels.BagViewModels
{
    public class GenericBagViewModel: INotifyPropertyChanged
    {
        public BagViewModel ParentViewModel { get; set; }
        public CommandExportSelected CommandExportSelected { get; set; }
        public CommandCopyAsUrl CommandCopyAsUrl { get; set; }
        public string ExportFilter { get; set; }

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

        private FileRecord _selectedRecord;
        public FileRecord SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                try
                {
                    _selectedRecord = value;
                    OnSelectedRecordChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                OnPropertyChanged();
            }
        }

        public GenericBagViewModel(BagViewModel parentViewModel)
        {
            this.ParentViewModel = parentViewModel;
            this.CommandExportSelected = new CommandExportSelected(this);
            this.CommandCopyAsUrl = new CommandCopyAsUrl(this);
            this.ExportFilter = "Raw File|*.*";

            var view = new RawResourceView();
            var viewModel = new SanBag.ViewModels.ResourceViewModels.RawResourceViewModel()
            {
                HexControl = view.HexEdit
            };

            CurrentResourceView = view;
            CurrentResourceView.DataContext = viewModel;
        }

        public virtual bool IsValidRecord(FileRecord record)
        {
            return true;
        }

        internal void ExportRecords(List<FileRecord> recordsToExport)
        {
            if (recordsToExport.Count == 0)
            {
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Filter = ExportFilter;

            if (recordsToExport.Count == 1)
            {
                dialog.FileName = recordsToExport[0].Name;
            }
            else
            {
                dialog.FileName = "Multiple Files";
            }

            if (dialog.ShowDialog() == true)
            {
                var outputDirectory = Path.GetDirectoryName(dialog.FileName);
                var fileExtension = Path.GetExtension(dialog.FileName);

                var exportViewModel = new ExportViewModel
                {
                    RecordsToExport = recordsToExport,
                    BagPath = ParentViewModel.BagPath,
                    OutputDirectory = outputDirectory,
                    FileExtension = fileExtension,
                    CustomSaveFunc = OnExportFile
                };

                var exportDialog = new ExportView
                {
                    DataContext = exportViewModel
                };
                exportDialog.ShowDialog();
            }
        }

        private static void ExportRawFile(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));

            using (var outStream = File.OpenWrite(outputPath))
            {
                exportParameters.FileRecord.Save(exportParameters.BagStream, outStream, exportParameters.OnProgressReport, exportParameters.ShouldCancel);
            }
        }

        private void OnExportFile(ExportParameters exportParameters)
        {
            var recordExtension = Path.GetExtension(exportParameters.FileRecord.Name);
            if (string.Equals(recordExtension, exportParameters.FileExtension, StringComparison.OrdinalIgnoreCase))
            {
                ExportRawFile(exportParameters);
            }
            else
            {
                CustomFileExport(exportParameters);
            }
        }

        protected virtual void CustomFileExport(ExportParameters exportParameters)
        {
            ExportRawFile(exportParameters);
        }

        public static void CopyAsUrl(FileRecord fileRecord)
        {
            if (fileRecord != null)
            {
                Clipboard.SetText($"https://sansar-asset-production.s3-us-west-2.amazonaws.com/{fileRecord.Name}");
            }
        }

        protected virtual void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ResourceViewModels.RawResourceViewModel;
            if (view == null)
            {
                return;
            }

            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                view.InitFromRecord(bagStream, SelectedRecord);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
