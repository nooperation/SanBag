using LibSanBag;
using SanBag.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SanBag.Commands
{
    public class CommandCancelExport : ICommand
    {
        private ExportViewModel viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandCancelExport(ExportViewModel viewModel)
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
