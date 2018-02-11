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
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SanBag.Views.ResourceViews;

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
            ExportFilter += "|DDS Source Image|*.dds|PNG Image|*.png|JPG Image|*.jpg|BMP Image|*.bmp|GIF Image|*.gif";
            CurrentResourceView = new TextureResourceView();
            CurrentResourceView.DataContext = new SanBag.ViewModels.ResourceViewModels.TextureResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.TextureResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        public static void Export(TextureResource resource, Stream outStream, string fileExtension)
        {
            if (string.Equals(fileExtension, ".dds", StringComparison.CurrentCultureIgnoreCase))
            {
                var imageBytes = resource.DdsBytes;
                outStream.Write(imageBytes, 0, imageBytes.Length);
            }
            else
            {
                var codec = LibDDS.ConversionOptions.CodecType.CODEC_JPEG;
                switch (fileExtension)
                {
                    case ".png":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_PNG;
                        break;
                    case ".jpg":
                    case ".jpeg":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_JPEG;
                        break;
                    case ".gif":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_GIF;
                        break;
                    case ".bmp":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_BMP;
                        break;
                    case ".ico":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_ICO;
                        break;
                    case ".wmp":
                        codec = LibDDS.ConversionOptions.CodecType.CODEC_WMP;
                        break;
                }
                var imageBytes = resource.ConvertTo(codec);
                outStream.Write(imageBytes, 0, imageBytes.Length);
            }
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
            var view = CurrentResourceView.DataContext as ResourceViewModels.TextureResourceViewModel;
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
