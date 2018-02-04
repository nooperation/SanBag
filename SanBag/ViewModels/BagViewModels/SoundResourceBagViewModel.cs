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
using SanBag.Views.ResourceViews;

namespace SanBag.ViewModels.BagViewModels
{
    class SoundResourceBagViewModel : GenericBagViewModel
    {
        public SoundResourceBagViewModel(BagViewModel parentViewModel) : base(parentViewModel)
        {
            ExportFilter += "|Wav Sound|*.wav";
            CurrentResourceView = new SoundResourceView();
            CurrentResourceView.DataContext = new SanBag.ViewModels.ResourceViewModels.SoundResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.SoundResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            var soundResource = SoundResource.Create();
            soundResource.InitFromRecord(exportParameters.BagStream, exportParameters.FileRecord);

            LibFSB.SaveAs(soundResource.SoundBytes, outputPath);
            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ResourceViewModels.SoundResourceViewModel;
            if (view == null)
            {
                return;
            }

            view.Unload();
            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                view.InitFromRecord(bagStream, SelectedRecord);
            }
        }
    }
}
