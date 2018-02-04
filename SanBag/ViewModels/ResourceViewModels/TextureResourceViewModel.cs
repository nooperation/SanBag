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
using SanBag.Annotations;
using SanBag.Commands;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.ViewModels.ResourceViewModels
{
    class TextureResourceViewModel : BaseViewModel, ISavable
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

        protected override void LoadFromStream(Stream resourceStream)
        {
            _currentResource = TextureResource.Create();
            _currentResource.InitFromStream(resourceStream);

            var imageBytes = LibDDS.GetImageBytesFromDds(_currentResource.DdsBytes, 256, 256);
            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = new MemoryStream(imageBytes);
            newImage.EndInit();

            CurrentImage = newImage;
        }

        public void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(Name) + ".dds";
            dialog.Filter = "DDS Source Image|*.dds|PNG Image|*.png|JPG Image|*.jpg|BMP Image|*.bmp|GIF Image|*.gif";
            if (dialog.ShowDialog() == true)
            {
                using (var outFile = File.OpenWrite(dialog.FileName))
                {
                    TextureResourceBagViewModel.Export(_currentResource, outFile, Path.GetExtension(dialog.FileName).ToLower());
                }
            }
        }
    }
}
