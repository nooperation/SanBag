using LibSanBag;
using SanBag.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    public class CommandManifestCancelExport : ICommand
    {
        private ExportManifestViewModel viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandManifestCancelExport(ExportManifestViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            viewModel.CancelExport();
        }
    }
}
