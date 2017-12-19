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
    class GeometryResourceBagViewModel : GenericBagViewModel
    {
        public GeometryResourceBagViewModel(BagViewModel parentViewModel) : base(parentViewModel)
        {
            ExportFilter += "|Obj File|*.obj";
            CurrentResourceView = new GeometryResourceView();
            CurrentResourceView.DataContext = new SanBag.ViewModels.ResourceViewModels.GeometryResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.GeometryResourceResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            throw new NotImplementedException();
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ResourceViewModels.GeometryResourceViewModel;
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
