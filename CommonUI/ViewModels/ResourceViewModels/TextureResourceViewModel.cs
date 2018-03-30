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

            var imageBytes = _currentResource.ConvertTo(TextureResource.TextureType.PNG);
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
                    // TODO: TextureResourceBagViewModel.Export(_currentResource, outFile, Path.GetExtension(dialog.FileName).ToLower());
                }
            }
        }
    }
}
