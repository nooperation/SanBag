using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using SanBag.Models;

namespace SanBag.ViewModels.BagViewModels
{
    class SoundResourceViewModel : GenericBagViewModel
    {
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

        protected override void OnSelectedRecordChanged()
        {
            if (CurrentResourceView != null && CurrentResourceView.DataContext != null)
            {
                var previousModel = CurrentResourceView.DataContext as ResourceViewModels.SoundResourceViewModel;
                previousModel.Unload();
            }
            var newViewModel = new SanBag.ViewModels.ResourceViewModels.SoundResourceViewModel();
            CurrentResourceView = new SanBag.Views.ResourceViews.SoundResourceView()
            {
                DataContext = newViewModel
            };

            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                using (var soundResourceStream = new MemoryStream())
                {
                    SelectedRecord.Save(bagStream, soundResourceStream);
                    bagStream.Close();

                    soundResourceStream.Seek(0L, SeekOrigin.Begin);
                    newViewModel.ReloadFromStream(soundResourceStream);
                }
            }
        }

        public SoundResourceViewModel(BagViewModel parentViewModel) : base(parentViewModel)
        {
            ExportFilter += "|Wav Sound|*.wav";
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.SoundResource;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            var soundResource = new SoundResource(exportParameters.BagStream, exportParameters.FileRecord);
            LibFSB.SaveAs(soundResource.SoundBytes, outputPath);
            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }
    }
}
