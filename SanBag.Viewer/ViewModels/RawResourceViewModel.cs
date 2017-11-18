using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibSanBag.ResourceUtils;
using SanBag.Viewer.Views;

namespace SanBag.Viewer.ViewModels
{
    class RawResourceViewModel : BaseViewModel
    {
        public WpfHexaEditor.HexEditor HexControl { get; set; }

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

        public override void Reload()
        {
            try
            {
                DecompressedBytes = OodleLz.DecompressResource(CurrentPath);
            }
            catch (Exception ex)
            {
                DecompressedBytes = File.ReadAllBytes(CurrentPath);
            }
        }
    }
}
