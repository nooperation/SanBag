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
    public class CommandExportSelected : ICommand
    {
        private GenericBagViewModel viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandExportSelected( GenericBagViewModel viewModel)
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
            var fileRecords = items.Cast<FileRecord>().ToList();

            viewModel.ExportRecords(fileRecords);
        }
    }
}
