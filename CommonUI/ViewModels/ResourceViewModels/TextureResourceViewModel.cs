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
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using CommonUI.Annotations;
using CommonUI.Commands;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class TextureResourceViewModel : BaseViewModel, ISavable
    {
        private TextureResource _currentResource;
        public CommandSaveAs CommandSaveAs { get; set; }

        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        public TextureResourceViewModel()
        {
            CommandSaveAs = new CommandSaveAs(this);
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            _currentResource = TextureResource.Create(version);
            _currentResource.InitFromStream(resourceStream);

            if (_currentResource.SourceType == TextureResource.TextureType.CRN)
            {
                var imageBytes = _currentResource.ConvertTo(TextureResource.TextureType.JPG);
                var newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.StreamSource = new MemoryStream(imageBytes);
                newImage.EndInit();

                CurrentImage = newImage;
            }
            else
            {
                var ddsBytes = _currentResource.CompressedTextureBytes;

                var imageBytes = LibDDS.GetImageBytesFromDds(ddsBytes, 256, 256);
                var newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.StreamSource = new MemoryStream(imageBytes);
                newImage.EndInit();

                CurrentImage = newImage;
            }
        }

        public static void Export(TextureResource resource, Stream outStream, string fileExtension)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }
            if (fileExtension == null)
            {
                throw new ArgumentNullException(nameof(fileExtension));
            }

            byte[] ddsBytes;
            if (resource.SourceType == TextureResource.TextureType.CRN)
            {
                ddsBytes = resource.ConvertTo(TextureResource.TextureType.DDS);
            }
            else
            {
                ddsBytes = resource.CompressedTextureBytes;
            }

            LibDDS.ConversionOptions.CodecType codec;
            switch (fileExtension.ToLower())
            {
                case ".png":
                    codec = LibDDS.ConversionOptions.CodecType.CODEC_PNG;
                    break;
                case ".jpg":
                    codec = LibDDS.ConversionOptions.CodecType.CODEC_JPEG;
                    break;
                case ".bmp":
                    codec = LibDDS.ConversionOptions.CodecType.CODEC_BMP;
                    break;
                default:
                case ".dds":
                    codec = LibDDS.ConversionOptions.CodecType.CODEC_DDS;
                    break;
            }

            byte[] imageBytes;
            if (codec != LibDDS.ConversionOptions.CodecType.CODEC_DDS)
            {
                imageBytes = LibDDS.GetImageBytesFromDds(ddsBytes, 0, 0, codec);
            }
            else
            {
                imageBytes = ddsBytes;
            }

            outStream.Write(imageBytes, 0, imageBytes.Length);
        }

        public void SaveAs()
        {
            if (_currentResource == null)
            {
                MessageBox.Show("Attempting to export a null texture resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(Name) + ".dds";
            dialog.Filter = "DDS Source Image|*.dds|PNG Image|*.png|JPG Image|*.jpg|BMP Image|*.bmp";
            if (dialog.ShowDialog() == true)
            {
                using (var outFile = File.OpenWrite(dialog.FileName))
                {
                    Export(_currentResource, outFile, Path.GetExtension(dialog.FileName).ToLower());
                }
            }
        }
    }
}
