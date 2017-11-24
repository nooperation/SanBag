using LibSanBag;
using System;
using System.Windows.Input;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    public class CommandCopyAsUrl : ICommand
    {
        private readonly GenericBagViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandCopyAsUrl(GenericBagViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            var fileRecord = parameter as FileRecord;
            GenericBagViewModel.CopyAsUrl(fileRecord);
        }
    }
}
