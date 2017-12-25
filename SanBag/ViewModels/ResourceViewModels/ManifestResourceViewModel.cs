using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.Providers;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.ViewModels.BagViewModels;
using SanBag.Views.BagViews;
using ExportView = SanBag.Views.ExportView;

namespace SanBag.ViewModels.ResourceViewModels
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

        public ManifestResourceViewModel()
        {
            Filters.Add(FilterNone);
            CurrentFilter = FilterNone;
            CommandManifestExportSelected = new CommandManifestExportSelected(this);
        }

        protected override void LoadFromStream(Stream resourceStream)
        {
            var resource = new ManifestResource();
            resource.InitFromStream(resourceStream);
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

        protected void CustomFileExport(ExportParameters exportParameters)
        {
            var payloadTypes = new List<FileRecordInfo.PayloadType> {
                FileRecordInfo.PayloadType.Payload,
                FileRecordInfo.PayloadType.Manifest
            };
            var assetType = FileRecordInfo.GetResourceType(exportParameters.FileRecord.Name);

            foreach (var payloadType in payloadTypes)
            {
                try
                {
                    var downloadTask = FileRecordInfo.DownloadResourceAsync(
                        exportParameters.FileRecord.Info.Hash.ToLower(),
                        assetType,
                        payloadType,
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
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR downloading resource {exportParameters.FileRecord.Info.Hash}.{assetType}.{payloadType}: {ex.Message}");
                    continue;
                }
            }
        }
    }
}
