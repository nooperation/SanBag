using LibSanBag;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SanBag.ViewModels
{
    public class GenericBagViewModel
    {
        public MainViewModel ParentViewModel { get; set; }
        public CommandExportSelected CommandExportSelected { get; set; }
        public CommandCopyAsUrl CommandCopyAsUrl { get; set; }
        public string ExportFilter { get; set; }

        public GenericBagViewModel(MainViewModel parentViewModel)
        {
            this.ParentViewModel = parentViewModel;
            this.CommandExportSelected = new CommandExportSelected(this);
            this.CommandCopyAsUrl = new CommandCopyAsUrl(this);
            this.ExportFilter = "Raw File|*.*";
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
    }
}
