using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSanBag.FileResources;
using SanBag.Commands;

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
            //CommandManifestExportSelected = new CommandManifestExportSelected(this);
        }

        protected override void LoadFromStream(Stream resourceStream)
        {
            var resource = new ManifestResource();
            resource.InitFromStream(resourceStream);
            ManifestList = resource.Entries;
            ManifestList = resource.Entries;

            //FileName = resource.Filename;
            //SourceCode = resource.Source;
        }


        //public void OnSelectedRecordChanged()
        //{
        //    try
        //    {
        //        using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
        //        {
        //            var manifest = new ManifestResource();
        //            manifest.InitFromRecord(bagStream, SelectedRecord);
        //            ManifestList = manifest.Entries;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        ManifestList = new List<ManifestEntry>();
        //    }
        //}
    }
}
