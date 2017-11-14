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
using static LibSanBag.FileResources.ManifestResource;

namespace SanBag.ViewModels
{
    public class ManifestViewModel : GenericBagViewModel, INotifyPropertyChanged
    {
        public readonly string FilterNone = "None";

        public CommandManifestExportSelected CommandManifestExportSelected { get; set; }

        private FileRecord _selectedRecord;
        public FileRecord SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                _selectedRecord = value;
                UpdatePreview();
                OnPropertyChanged();
            }
        }

        private List<ManifestEntry> _manifestList = new List<ManifestEntry>();

        public List<ManifestEntry> ManifestList
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

        public List<ManifestEntry> FilteredManifestList
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

        public ManifestViewModel(MainViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|Manifest Dump|*.txt";

            Filters.Add(FilterNone);
            CurrentFilter = FilterNone;
            CommandManifestExportSelected = new CommandManifestExportSelected(this);
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Name.IndexOf("manifest", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void UpdatePreview()
        {
            try
            {
                using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
                {
                    var manifest = new ManifestResource(bagStream, SelectedRecord);
                    ManifestList = manifest.Entries;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ManifestList = new List<ManifestEntry>();
            }
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            ManifestResource manifest;
            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                manifest = new ManifestResource(bagStream, SelectedRecord);
                ManifestList = manifest.Entries;
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

        internal void ExportRecords(List<ManifestEntry> recordsToExport)
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

                var exportManifestViewModel = new ExportManifestViewModel
                {
                    RecordsToExport = recordsToExport,
                    OutputDirectory = outputDirectory,
                };

                var exportDialog = new ExportManifestView
                {
                    DataContext = exportManifestViewModel
                };
                exportDialog.ShowDialog();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}