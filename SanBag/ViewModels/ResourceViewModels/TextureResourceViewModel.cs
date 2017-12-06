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
using LibSanBag.ResourceUtils;
using SanBag.Annotations;

namespace SanBag.ViewModels.ResourceViewModels
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
            using (var compressedStream = File.OpenRead(CurrentPath))
            {
                ReloadFromStream(compressedStream);
            }
        }

        public override void ReloadFromStream(Stream resourceStream)
        {
            var resource = new TextureResource();
            resource.InitFromStream(resourceStream);

            var imageBytes = LibDDS.GetImageBytesFromDds(resource.DdsBytes, 256, 256);
            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = new MemoryStream(imageBytes);
            newImage.EndInit();

            CurrentImage = newImage;
        }
    }
}
