using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Views;

namespace SanBag.ViewModels.ResourceViewModels
{
    class RawResourceViewModel : BaseViewModel, ISavable
    {
        public WpfHexaEditor.HexEditor HexControl { get; set; }
        public CommandSaveAs CommandSaveAs { get; set; }
        public CommandExit CommandExit { get; set; } = new CommandExit();

        private byte[] _decompressedBytes;
        public byte[] DecompressedBytes
        {
            get => _decompressedBytes;
            set
            {
                _decompressedBytes = value;
                DecompressedStream = new MemoryStream(_decompressedBytes);
                OnPropertyChanged();
            }
        }

        private MemoryStream _decompressedStream = new MemoryStream();
        public MemoryStream DecompressedStream
        {
            get => _decompressedStream;
            set
            {
                _decompressedStream.Dispose();
                _decompressedStream = value;
                HexControl.Stream = value;
                OnPropertyChanged();
            }
        }

        public RawResourceViewModel()
        {
            CommandSaveAs = new CommandSaveAs(this);
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            try
            {
                DecompressedBytes = OodleLz.DecompressResource(resourceStream);
            }
            catch (Exception)
            {
                resourceStream.Seek(0, SeekOrigin.Begin);
                using (var decompressedBytesStream = new MemoryStream())
                {
                    resourceStream.CopyTo(decompressedBytesStream);
                    DecompressedBytes = decompressedBytesStream.ToArray();
                }
            }
        }

        public void SaveAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(Name) + ".bin";
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllBytes(dialog.FileName, _decompressedBytes);
                MessageBox.Show($"Successfully wrote {_decompressedBytes.Length} byte(s) to {dialog.FileName}");
            }
        }
    }
}
