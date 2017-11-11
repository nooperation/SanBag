﻿using LibSanBag;
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

namespace SanBag.ViewModels
{
    public class LuaScriptResourceViewModel : GenericBagViewModel, INotifyPropertyChanged
    {
        private FileRecord _selectedRecord;
        public FileRecord SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                _selectedRecord = value;
                UpdatePreviewText();
                OnPropertyChanged();
            }
        }

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

        public LuaScriptResourceViewModel(MainViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|Lua File|*.lua";
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.LuaScriptResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        private void UpdatePreviewText()
        {
            try
            {
                using (var bagStream = File.OpenRead(ParentViewModel.BagPath))
                {
                    var scriptSourceText = new LuaScriptResource(bagStream, SelectedRecord);
                    PreviewCode = scriptSourceText.Source;
                }
            }
            catch (Exception)
            {
                PreviewCode = "";
            }
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var scriptCompiledBytecode = new LuaScriptResource(exportParameters.BagStream, exportParameters.FileRecord);
            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));
            File.WriteAllText(outputPath, scriptCompiledBytecode.Source);

            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}