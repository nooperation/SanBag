using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using CommonUI.Models;
using CommonUI.Views.ResourceViews;
using CommonUI.ViewModels.ResourceViewModels;

namespace SanBag.ViewModels.BagViewModels
{
    class GeometryResourceBagViewModel : GenericBagViewModel
    {
        public GeometryResourceBagViewModel(BagViewModel parentViewModel) : base(parentViewModel)
        {
            //ExportFilter += "|Fbx File (Download)|*.fbx";
            CurrentResourceView = new GeometryResourceView();
            CurrentResourceView.DataContext = new GeometryResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.GeometryResourceResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            if (exportParameters.FileExtension == ".fbx")
            {
                var downloadManifestResult = FileRecordInfo.DownloadResourceAsync(
                    exportParameters.FileRecord.Info.Hash,
                    FileRecordInfo.ResourceType.GeometryResourceImport,
                    FileRecordInfo.PayloadType.Manifest,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider()
                ).Result;

                var manifest = ManifestResource.Create();
                manifest.InitFromRawDecompressed(downloadManifestResult.Bytes);
                var sourceGeometryEntry = manifest.Entries.Find(n => n.Name.Contains("Canonical"));
                if (sourceGeometryEntry == default(ManifestResource.ManifestEntry))
                {
                    throw new Exception("Canonical resource not found in import manifest");
                }

                var downloadGeometryResult = FileRecordInfo.DownloadResourceAsync(
                    sourceGeometryEntry.HashString,
                    FileRecordInfo.ResourceType.GeometryResourceCanonical,
                    FileRecordInfo.PayloadType.Payload,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider()
                ).Result;

                var geometryCanonical = GeometryResourceCanonical.Create(downloadGeometryResult.Version);
                geometryCanonical.InitFromRawCompressed(downloadGeometryResult.Bytes);

                var outputName = Path.GetFileName(sourceGeometryEntry.HashString + sourceGeometryEntry.Name + ".fbx");
                File.WriteAllBytes(
                    Path.Combine(exportParameters.OutputDirectory, outputName),
                    geometryCanonical.Content
                );
            }
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as CommonUI.ViewModels.ResourceViewModels.GeometryResourceViewModel;
            if (view == null)
            {
                return;
            }

            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                view.InitFromRecord(bagStream, SelectedRecord);
            }
        }
    }
}
