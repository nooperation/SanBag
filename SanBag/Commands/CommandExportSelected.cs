using LibSanBag;
using System;
using System.Linq;
using System.Windows.Input;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    public class CommandExportSelected : ICommand
    {
        private readonly GenericBagViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandExportSelected( GenericBagViewModel viewModel)
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
            var fileRecords = items.Cast<FileRecord>().ToList();

            _viewModel.ExportRecords(fileRecords);
        }
    }
}
