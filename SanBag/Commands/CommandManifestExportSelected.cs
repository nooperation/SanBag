using System;
using System.Linq;
using System.Windows.Input;
using static LibSanBag.FileResources.ManifestResource;
using SanBag.ViewModels.BagViewModels;
using SanBag.ViewModels.ResourceViewModels;

namespace SanBag.Commands
{
    public class CommandManifestExportSelected : ICommand
    {
        private readonly ManifestResourceViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandManifestExportSelected(ManifestResourceViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            var items = parameter as System.Collections.IList;
            var manifestEntries = items.Cast<ManifestEntry>().ToList();

            _viewModel.ExportRecords(manifestEntries);
        }
    }
}
