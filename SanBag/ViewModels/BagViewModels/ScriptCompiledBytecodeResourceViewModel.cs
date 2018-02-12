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

namespace SanBag.ViewModels.BagViewModels
{
    public class ScriptCompiledBytecodeResourceViewModel : GenericBagViewModel
    {
        public ScriptCompiledBytecodeResourceViewModel(BagViewModel parentViewModel)
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
            var scriptCompiledBytecode = ScriptCompiledBytecodeResource.Create(exportParameters.FileRecord.Info?.VersionHash ?? string.Empty);
            scriptCompiledBytecode.InitFromRecord(exportParameters.BagStream, exportParameters.FileRecord);
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            File.WriteAllBytes(outputPath, scriptCompiledBytecode.AssemblyBytes);

            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }
    }
}
