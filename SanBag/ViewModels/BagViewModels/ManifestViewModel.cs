using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.ResourceUtils;
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
using LibSanBag.Providers;
using SanBag.Views.BagViews;
using static LibSanBag.FileResources.ManifestResource;

namespace SanBag.ViewModels.BagViewModels
{
    public class ManifestViewModel : GenericBagViewModel, INotifyPropertyChanged
    {
 
        public ManifestViewModel(BagViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|Manifest Dump|*.txt";
            CurrentResourceView = new SanBag.Views.ResourceViews.ManifestResourceView();
            CurrentResourceView.DataContext = new SanBag.ViewModels.ResourceViewModels.ManifestResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Name.IndexOf("manifest", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ResourceViewModels.ManifestResourceViewModel;
            if (view == null)
            {
                return;
            }

            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                view.InitFromRecord(bagStream, SelectedRecord);
            }
        }


        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var payloadTypes = new List<FileRecordInfo.PayloadType> { FileRecordInfo.PayloadType.Payload, FileRecordInfo.PayloadType.Manifest };
            var assetType = FileRecordInfo.GetResourceType(exportParameters.FileRecord.Name);
            var assetVersions = AssetVersions.GetResourceVersions(assetType);

            foreach (var payloadType in payloadTypes)
            {
                FileRecordInfo.DownloadResults downloadResult;
                try
                {
                    downloadResult = FileRecordInfo.DownloadResourceAsync(exportParameters.FileRecord.Name.ToLower(), assetType, payloadType, FileRecordInfo.VariantType.NoVariants, new HttpClientProvider()).Result;
                    if (downloadResult != null)
                    {
                        var outputPath = Path.Combine(exportParameters.OutputDirectory, downloadResult.Name);

                        using (var out_stream = File.OpenWrite(outputPath))
                        {
                            out_stream.Write(downloadResult.Bytes, 0, downloadResult.Bytes.Length);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            //ManifestResource manifest;
            //using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            //{
            //    manifest = new ManifestResource();
            //    manifest.InitFromRecord(bagStream, SelectedRecord);
            //    ManifestList = manifest.Entries;
            //}
            //
            //var sb = new StringBuilder();
            //sb.AppendLine("Entries:");
            //foreach (var item in manifest.Entries)
            //{
            //    sb.AppendLine($"  {item.HashString}.{item.Name}");
            //}
            //sb.AppendLine();
            //sb.AppendLine("HashList:");
            //foreach (var item in manifest.HashList)
            //{
            //    sb.AppendLine($"  {item}");
            //}
            //sb.AppendLine();
            //sb.AppendLine("Unknown A:");
            //foreach (var item in manifest.UnknownListA)
            //{
            //    sb.AppendLine($" {item.Item1} {item.Item2} {item.Item3}");
            //}
            //sb.AppendLine();
            //sb.AppendLine("Unknown B:");
            //foreach (var item in manifest.UnknownListB)
            //{
            //    sb.AppendLine($" {item}");
            //}
            //var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            //File.WriteAllText(outputPath, sb.ToString());
            //
            //exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        internal void ExportRecords(List<ManifestEntry> recordsToExport)
        {
            //if (recordsToExport.Count == 0)
            //{
            //    return;
            //}
            //
            //var dialog = new SaveFileDialog();
            //dialog.Filter = ExportFilter;
            //
            //if (recordsToExport.Count == 1)
            //{
            //    dialog.FileName = recordsToExport[0].Name;
            //}
            //else
            //{
            //    dialog.FileName = "Multiple Files";
            //}
            //
            //if (dialog.ShowDialog() == true)
            //{
            //    var outputDirectory = Path.GetDirectoryName(dialog.FileName);
            //
            //    var exportManifestViewModel = new ExportManifestViewModel
            //    {
            //        RecordsToExport = recordsToExport,
            //        OutputDirectory = outputDirectory,
            //    };
            //
            //    var exportDialog = new ExportManifestView
            //    {
            //        DataContext = exportManifestViewModel
            //    };
            //    exportDialog.ShowDialog();
            //}
        }
    }
}