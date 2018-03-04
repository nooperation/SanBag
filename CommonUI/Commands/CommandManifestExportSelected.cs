using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using static LibSanBag.FileResources.ManifestResource;
using CommonUI.ViewModels.ResourceViewModels;

namespace CommonUI.Commands
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
            _viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return parameter is IList;
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter is IList items)
            {
                var manifestEntries = items.Cast<ManifestEntry>().ToList();
                _viewModel.ExportRecords(manifestEntries);
            }
        }
    }
}
