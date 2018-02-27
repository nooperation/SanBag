using LibSanBag;
using Microsoft.Win32;
using CommonUI.Commands;
using CommonUI.Models;
using CommonUI.Views;
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
using CommonUI.ViewModels;
using LibSanBag.ResourceUtils;
using CommonUI.ViewModels.ResourceViewModels;
using CommonUI.Views.ResourceViews;
using SanBag.Commands;
using ExportView = CommonUI.Views.ExportView;

namespace SanBag.ViewModels.BagViewModels
{
    public class GenericBagViewModel : INotifyPropertyChanged
    {
        public BagViewModel ParentViewModel { get; set; }
        public CommandExportSelected CommandExportSelected { get; set; }
        public CommandCopyAsUrl CommandCopyAsUrl { get; set; }
        public string ExportFilter { get; set; }
        private Dictionary<FileRecordInfo.ResourceType, UserControl> ControlMap { get; }

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
                    if (value != null)
                    {
                        OnSelectedRecordChanged();
                    }
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

            ControlMap = new Dictionary<FileRecordInfo.ResourceType, UserControl>()
            {
                [FileRecordInfo.ResourceType.TextureResource] = new TextureResourceView
                {
                    DataContext = new TextureResourceViewModel()
                },
                [FileRecordInfo.ResourceType.ScriptSourceTextResource] = new ScriptSourceTextView()
                {
                    DataContext = new ScriptSourceTextViewModel()
                },
                [FileRecordInfo.ResourceType.LuaScriptResource] = new ScriptSourceTextView()
                {
                    DataContext = new ScriptSourceTextViewModel()
                },
                [FileRecordInfo.ResourceType.GeometryResourceResource] = new GeometryResourceView()
                {
                    DataContext = new GeometryResourceViewModel()
                },
                [FileRecordInfo.ResourceType.SoundResource] = new SoundResourceView()
                {
                    DataContext = new SoundResourceViewModel()
                },
            };

            var rawView = new RawResourceView();
            rawView.DataContext = new RawResourceViewModel(rawView);
            ControlMap[FileRecordInfo.ResourceType.Unknown] = rawView;
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

        public UserControl GetControlFor(FileRecordInfo recordInfo)
        {
            if (recordInfo != null)
            {
                if (recordInfo.IsRawImage)
                {
                    return new RawImageView
                    {
                        DataContext = new RawImageViewModel()
                    };
                }

                if (recordInfo.Payload == LibSanBag.FileRecordInfo.PayloadType.Manifest)
                {
                    return new ManifestResourceView
                    {
                        DataContext = new ManifestResourceViewModel()
                    };
                }

                if (ControlMap.ContainsKey(recordInfo.Resource))
                {
                    return ControlMap[recordInfo.Resource];
                }
            }

            return ControlMap[FileRecordInfo.ResourceType.Unknown];
        }

        private static void ExportRawFile(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name));

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
            if (SelectedRecord == null)
            {
                return;
            }

            try
            {
                var previousViewModel = CurrentResourceView?.DataContext as BaseViewModel;
                if (previousViewModel != null)
                {
                    previousViewModel.Unload();
                }

                CurrentResourceView = GetControlFor(SelectedRecord.Info);

                var currentViewModel = CurrentResourceView.DataContext as BaseViewModel;
                if (currentViewModel != null)
                {
                    using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
                    {
                        currentViewModel.InitFromRecord(bagStream, SelectedRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load resource: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
