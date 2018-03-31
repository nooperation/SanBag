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
    public class TextureResourceBagViewModel : GenericBagViewModel
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

        public TextureResourceBagViewModel(BagViewModel parentViewModel)
            : base(parentViewModel)
        {
            ExportFilter += "|DDS Source Image|*.dds|JPG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp";
            CurrentResourceView = new TextureResourceView();
            CurrentResourceView.DataContext = new TextureResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.TextureResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        public static void Export(TextureResource resource, Stream outStream, string fileExtension)
        {
            TextureResourceViewModel.Export(resource, outStream, fileExtension);
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));

            var textureResource = TextureResource.Create(exportParameters.FileRecord.Info?.VersionHash ?? string.Empty);
            textureResource.InitFromRecord(exportParameters.BagStream, exportParameters.FileRecord);
            using (var outFile = File.OpenWrite(outputPath))
            {
                Export(textureResource, outFile, exportParameters.FileExtension);
            }
            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as TextureResourceViewModel;
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
