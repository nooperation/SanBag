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

            if (_currentResource.Resource.Data[0] == 'D' && _currentResource.Resource.Data[1] == 'D' && _currentResource.Resource.Data[2] == 'S')
            {
                var imageBytes = LibDDS.GetImageBytesFromDds(_currentResource.Resource.Data, 256, 256);
                var newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.StreamSource = new MemoryStream(imageBytes);
                newImage.EndInit();

                CurrentImage = newImage;
            }
            else
            {
                var imageBytes = ConvertTo(_currentResource, TextureType.JPG);
                var newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.StreamSource = new MemoryStream(imageBytes);
                newImage.EndInit();

                CurrentImage = newImage;
            }
        }

        public enum TextureType
        {
            DDS = 0,
            CRN = 1,
            PNG = 2,
            BMP = 3,
            JPG = 4
        }

        public static LibDDS.ConversionOptions.CodecType GetDdsTextureType(TextureType codec)
        {
            switch (codec)
            {
                case TextureType.DDS:
                    return LibDDS.ConversionOptions.CodecType.CODEC_DDS;
                case TextureType.PNG:
                    return LibDDS.ConversionOptions.CodecType.CODEC_PNG;
                case TextureType.BMP:
                    return LibDDS.ConversionOptions.CodecType.CODEC_BMP;
                default:
                    throw new NotImplementedException("Cannot convert DDS to " + codec.ToString());
            }
        }

        public static LibCRN.ImageCodec GetCrnTextureType(TextureType codec)
        {
            switch (codec)
            {
                case TextureType.DDS:
                    return LibCRN.ImageCodec.DDS;
                case TextureType.CRN:
                    return LibCRN.ImageCodec.CRN;
                case TextureType.PNG:
                    return LibCRN.ImageCodec.PNG;
                case TextureType.BMP:
                    return LibCRN.ImageCodec.BMP;
                case TextureType.JPG:
                    return LibCRN.ImageCodec.JPG;
                default:
                    throw new NotImplementedException("Cannot convert CRN to " + codec.ToString());
            }
        }

        /// <summary>
        /// Converts this texture to a different resolution, codec, or format
        /// </summary>
        /// <param name="codec">Type of image to convert this texture to</param>
        /// <param name="width">Width to resize image to. May not be available. Width of 0 preserves the original width.</param>
        /// <param name="height">Height to resize image to. May not be available. Height of 0 preserves the original height.</param>
        /// <returns>Converted image bytes</returns>
        public static byte[] ConvertTo(TextureResource resource, TextureType codec, UInt64 width = 0, UInt64 height = 0)
        {
            var textureBytes = resource.Resource.Data;

            if (textureBytes[0] == 'D' && textureBytes[1] == 'D' && textureBytes[2] == 'S')
            {
                if (codec == TextureType.DDS)
                {
                    return textureBytes;
                }

                var ddsCodec = GetDdsTextureType(codec);
                return LibDDS.GetImageBytesFromDds(textureBytes, width, height, ddsCodec);
            }
            else
            {
                if (codec == TextureType.CRN)
                {
                    return textureBytes;
                }

                var crnCodec = GetCrnTextureType(codec);
                return LibCRN.GetImageBytesFromCRN(textureBytes, crnCodec, resource.Resource.MipLevels.Count, null);
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

            TextureType codec;
            switch (fileExtension.ToLower())
            {
                case ".png":
                    codec = TextureType.PNG;
                    break;
                case ".jpg":
                    codec = TextureType.JPG;
                    break;
                case ".bmp":
                    codec = TextureType.BMP;
                    break;
                default:
                case ".dds":
                    codec = TextureType.DDS;
                    break;
            }

            var imageBytes = ConvertTo(resource, codec, 0, 0);
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
