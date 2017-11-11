using LibSanBag;
using LibSanBag.FileResources;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.ResourceUtils;
using SanBag.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SanBag.ViewModels
{
    public class ScriptCompiledBytecodeResourceViewModel : GenericBagViewModel, INotifyPropertyChanged
    {
        private FileRecord _selectedRecord;
        public FileRecord SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                _selectedRecord = value;
                OnPropertyChanged();
            }
        }

        public ScriptCompiledBytecodeResourceViewModel(MainViewModel parentViewModel)
            : base(parentViewModel)
        {
            ExportFilter += "|.Net Assembly|*.dll";
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.ScriptCompiledBytecodeResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var scriptCompiledBytecode = new ScriptCompiledBytecodeResource(exportParameters.BagStream, exportParameters.FileRecord);
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            File.WriteAllBytes(outputPath, scriptCompiledBytecode.AssemblyBytes);

            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
