using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
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
using System.Windows.Controls;
using SanBag.Views.ResourceViews;

namespace SanBag.ViewModels.BagViewModels
{
    public class ScriptSourceTextResourceViewModel : GenericBagViewModel
    {
        private string _previewCode = "";
        public string PreviewCode
        {
            get => _previewCode;
            set
            {
                _previewCode = value;
                OnPropertyChanged();
            }
        }

        public ScriptSourceTextResourceViewModel(BagViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|Script Source|*.cs";
            CurrentResourceView = new ScriptSourceTextView();
            CurrentResourceView.DataContext = new SanBag.ViewModels.ResourceViewModels.ScriptSourceTextViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.ScriptSourceTextResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var scriptCompiledBytecode = ScriptSourceTextResource.Create(exportParameters.FileRecord.Info?.VersionHash ?? string.Empty);
            scriptCompiledBytecode.InitFromRecord(exportParameters.BagStream, exportParameters.FileRecord);

            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            File.WriteAllText(outputPath, scriptCompiledBytecode.Source);

            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as ResourceViewModels.ScriptSourceTextViewModel;
            if (view == null)
            {
                return;
            }

            using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
            {
                view.InitFromRecord(bagStream, SelectedRecord);
            }
        }
    }
}