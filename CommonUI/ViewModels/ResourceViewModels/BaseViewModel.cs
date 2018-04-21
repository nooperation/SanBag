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
using CommonUI.Annotations;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Hash { get; set; }

        public void InitFromPath(string filePath)
        {
            Name = Path.GetFileName(filePath);

            var fileInfo = FileRecordInfo.Create(Name);
            Version = fileInfo?.VersionHash ?? string.Empty;
            Hash = fileInfo?.Hash ?? string.Empty;

            using (var fileStream = File.OpenRead(filePath))
            {
                LoadFromStream(fileStream, Hash);
            }
        }

        public void InitFromRecord(Stream sourceStream, FileRecord fileRecord)
        {
            Name = fileRecord.Name;
            using (var stream = new MemoryStream())
            {
                fileRecord.Save(sourceStream, stream);
                stream.Seek(0, SeekOrigin.Begin);
                Version = fileRecord.Info?.VersionHash ?? string.Empty;
                Hash = fileRecord.Info?.Hash ?? string.Empty;

                LoadFromStream(stream, Version);
            }
        }

        public void InitFromStream(Stream stream, string version="", string hash="")
        {
            Name = "Resource";
            Version = version ?? string.Empty;
            Hash = hash ?? string.Empty;

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
