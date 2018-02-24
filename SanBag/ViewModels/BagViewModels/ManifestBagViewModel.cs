using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using CommonUI.Commands;
using CommonUI.Models;
using CommonUI.ResourceUtils;
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
using LibSanBag.Providers;
using static LibSanBag.FileResources.ManifestResource;
using CommonUI.ViewModels.ResourceViewModels;

namespace SanBag.ViewModels.BagViewModels
{
    public class ManifestBagViewModel : GenericBagViewModel
    {
 
        public ManifestBagViewModel(BagViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|Manifest Dump|*.txt";
            CurrentResourceView = new CommonUI.Views.ResourceViews.ManifestResourceView();
            CurrentResourceView.DataContext = new ManifestResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Payload == FileRecordInfo.PayloadType.Manifest;
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ManifestResourceViewModel;
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
            ManifestResource manifest;
            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                manifest = ManifestResource.Create(exportParameters.FileRecord.Info?.VersionHash ?? string.Empty);
                manifest.InitFromRecord(bagStream, SelectedRecord);
            }

            var sb = new StringBuilder();
            sb.AppendLine("Entries:");
            foreach (var item in manifest.Entries)
            {
                sb.AppendLine($"  {item.HashString}.{item.Name}");
            }
            sb.AppendLine();
            sb.AppendLine("HashList:");
            foreach (var item in manifest.HashList)
            {
                sb.AppendLine($"  {item}");
            }
            sb.AppendLine();
            sb.AppendLine("Unknown A:");
            foreach (var item in manifest.UnknownListA)
            {
                sb.AppendLine($" {item.Item1} {item.Item2} {item.Item3}");
            }
            sb.AppendLine();
            sb.AppendLine("Unknown B:");
            foreach (var item in manifest.UnknownListB)
            {
                sb.AppendLine($" {item}");
            }
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            File.WriteAllText(outputPath, sb.ToString());
            
            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

    }
}