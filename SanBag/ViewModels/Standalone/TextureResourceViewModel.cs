using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LibSanBag.FileResources;
using SanBag.Annotations;

namespace SanBag.ViewModels.Standalone
{
    class TextureResourceViewModel : BaseViewModel
    {
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

        public override void Reload()
        {
            byte[] imageBytes = null;
            using (var compressedStream = File.OpenRead(CurrentPath))
            {
                var textureResource = new TextureResource(compressedStream);
                imageBytes = textureResource.ConvertTo(LibSanBag.ResourceUtils.LibDDS.ConversionOptions.CodecType.CODEC_JPEG);
            }
            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = new MemoryStream(imageBytes);
            newImage.EndInit();

            CurrentImage = newImage;
        }
    }
}
