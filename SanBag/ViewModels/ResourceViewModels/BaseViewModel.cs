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
            Name = Path.GetFileName(filePath);
            var version = FileRecordInfo.Create(Name)?.VersionHash ?? string.Empty;

            using (var fileStream = File.OpenRead(filePath))
            {
                LoadFromStream(fileStream, version);
            }
        }

        public void InitFromRecord(Stream sourceStream, FileRecord fileRecord)
        {
            Name = fileRecord.Name;
            using (var stream = new MemoryStream())
            {
                fileRecord.Save(sourceStream, stream);
                stream.Seek(0, SeekOrigin.Begin);
                LoadFromStream(stream, fileRecord.Info?.VersionHash ?? string.Empty);
            }
        }

        public void InitFromStream(Stream stream, string version="")
        {
            Name = "Resource";
            LoadFromStream(stream, version);
        }

        public virtual void Unload()
        {
        }

        protected abstract void LoadFromStream(Stream resourceStream, string version);

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
