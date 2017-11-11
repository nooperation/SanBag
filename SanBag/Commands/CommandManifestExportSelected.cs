using LibSanBag;
using SanBag.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static LibSanBag.FileResources.ManifestResource;

namespace SanBag.Commands
{
    public class CommandManifestExportSelected : ICommand
    {
        private ManifestViewModel viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandManifestExportSelected(ManifestViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            var items = parameter as System.Collections.IList;
            var manifestEntries = items.Cast<ManifestEntry>().ToList();

            viewModel.ExportRecords(manifestEntries);
        }
    }
}
