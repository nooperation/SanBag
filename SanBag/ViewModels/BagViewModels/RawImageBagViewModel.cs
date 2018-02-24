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
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CommonUI.Views.ResourceViews;
using CommonUI.ViewModels.ResourceViewModels;

namespace SanBag.ViewModels.BagViewModels
{
    public class RawImageBagViewModel : GenericBagViewModel
    {
        private BitmapImage _currentPreview;
        public BitmapImage PreviewImage
        {
            get => _currentPreview;
            set
            {
                _currentPreview = value;
                OnPropertyChanged();
            }
        }

        public RawImageBagViewModel(BagViewModel parentViewModel)
            : base(parentViewModel)
        {
            ExportFilter = "PNG Image|*.png";
            CurrentResourceView = new RawImageView();
            CurrentResourceView.DataContext = new RawImageViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.IsRawImage == true;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));

            using (var outFile = File.OpenWrite(outputPath))
            {
                exportParameters.FileRecord.Save(exportParameters.BagStream, outFile);
            }
            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as RawImageViewModel;
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
