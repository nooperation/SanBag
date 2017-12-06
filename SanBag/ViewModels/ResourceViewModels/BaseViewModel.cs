using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LibSanBag;
using LibSanBag.FileResources;
using SanBag.Annotations;

namespace SanBag.ViewModels.ResourceViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public void InitFromPath(string filePath)
        {
            Name = Path.GetFileNameWithoutExtension(filePath);
            using (var fileStream = File.OpenRead(filePath))
            {
                LoadFromStream(fileStream);
            }
        }

        public void InitFromRecord(Stream sourceStream, FileRecord fileRecord)
        {
            Name = fileRecord.Name;
            using (var stream = new MemoryStream())
            {
                fileRecord.Save(sourceStream, stream);
                LoadFromStream(stream);
            }
        }

        public void InitFromStream(Stream stream)
        {
            Name = "Resource";
            LoadFromStream(stream);
        }

        protected abstract void LoadFromStream(Stream resourceStream);

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
