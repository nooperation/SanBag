﻿using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using CommonUI.Commands;
using CommonUI.Models;
using CommonUI.ResourceUtils;
using CommonUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommonUI.Views.ResourceViews;
using CommonUI.ViewModels.ResourceViewModels;
using Newtonsoft.Json;

namespace SanBag.ViewModels.BagViewModels
{
    public class BlueprintResourceBagViewModel : GenericBagViewModel
    {
        public BlueprintResourceBagViewModel(BagViewModel parentViewModel)
                : base(parentViewModel)
        {
            ExportFilter += "|JSON File|*.json";
            CurrentResourceView = new BlueprintResourceView();
            CurrentResourceView.DataContext = new BlueprintResourceViewModel();
        }

        public override bool IsValidRecord(FileRecord record)
        {
            return record.Info?.Resource == FileRecordInfo.ResourceType.BlueprintResource &&
                   record.Info?.Payload == FileRecordInfo.PayloadType.Payload;
        }

        protected override void CustomFileExport(ExportParameters exportParameters)
        {
            var Blueprint = BlueprintResource.Create();
            Blueprint.InitFromRecord(exportParameters.BagStream, exportParameters.FileRecord);

            var outputPath = Path.GetFullPath(Path.Combine(exportParameters.OutputDirectory, exportParameters.FileRecord.Name + exportParameters.FileExtension));

            var text = JsonConvert.SerializeObject(Blueprint, Formatting.Indented);
            File.WriteAllText(outputPath, text);

            exportParameters.OnProgressReport?.Invoke(exportParameters.FileRecord, 0);
        }

        protected override void OnSelectedRecordChanged()
        {
            var view = CurrentResourceView.DataContext as BlueprintResourceViewModel;
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