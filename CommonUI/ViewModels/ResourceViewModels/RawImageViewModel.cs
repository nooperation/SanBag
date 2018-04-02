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
    public class RawImageViewModel : BaseViewModel, ISavable
    {
        [CanBeNull]
        private byte[] ImageBytes { get; set; }

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

        public RawImageViewModel()
        {
            CommandSaveAs = new CommandSaveAs(this);
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            ImageBytes = new byte[resourceStream.Length];
            resourceStream.Read(ImageBytes, 0, (int)resourceStream.Length);

            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = new MemoryStream(ImageBytes);
            newImage.EndInit();

            CurrentImage = newImage;
        }

        public void SaveAs()
        {
            if (ImageBytes == null)
            {
                MessageBox.Show("Attempting to export a null image resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(Name);
            dialog.Filter = "PNG Image|*.png";
            if (dialog.ShowDialog() == true)
            {
                using (var outFile = File.OpenWrite(dialog.FileName))
                {
                    outFile.Write(ImageBytes, 0, ImageBytes.Length);
                }
            }
        }
    }
}
