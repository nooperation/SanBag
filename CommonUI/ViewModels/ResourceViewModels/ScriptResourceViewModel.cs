using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommonUI.Views.ResourceViews;
using LibSanBag.FileResources;
using LibSanBag.Providers;

namespace CommonUI.ViewModels.ResourceViewModels
{
    class ScriptResourceViewModel : BaseViewModel
    {
        private ScriptSourceTextViewModel _currentScriptSourceTextViewModel;
        public ScriptSourceTextViewModel CurrentScriptSourceTextViewModel
        {
            get => _currentScriptSourceTextViewModel;
            set
            {
                _currentScriptSourceTextViewModel?.Unload();
                _currentScriptSourceTextViewModel = value;
                OnPropertyChanged();
            }
        }

        private ScriptMetadataResourceViewModel _currentScriptMetadataResourceViewModel;
        public ScriptMetadataResourceViewModel CurrentScriptMetadataResourceViewModel
        {
            get => _currentScriptMetadataResourceViewModel;
            set
            {
                _currentScriptMetadataResourceViewModel?.Unload();
                _currentScriptMetadataResourceViewModel = value;
                OnPropertyChanged();
            }
        }

        private int _currentTabIndex;
        public int CurrentTabIndex
        {
            get => _currentTabIndex;
            set
            {
                _currentTabIndex = value;
                if (_currentTabIndex == 1)
                {
                    // DownloadSource();
                }
                OnPropertyChanged();
            }
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var resourceType = LibSanBag.AssetVersions.GetResourceTypeFromVersion(version);
            if (resourceType == LibSanBag.FileRecordInfo.ResourceType.ScriptMetadataResource)
            {
                var viewModel = new ScriptMetadataResourceViewModel();
                viewModel.InitFromStream(resourceStream, version);
                CurrentScriptMetadataResourceViewModel = viewModel;
            }
            else if (resourceType == LibSanBag.FileRecordInfo.ResourceType.ScriptSourceTextResource)
            {
                var viewModel = new ScriptSourceTextViewModel();
                viewModel.InitFromStream(resourceStream, version);
                CurrentScriptSourceTextViewModel = viewModel;
            }
        }
    }
}
