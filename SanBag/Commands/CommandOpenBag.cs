using SanBag.ViewModels;
using System;
using System.Windows.Input;

namespace SanBag.Commands
{
    public class CommandOpenBag : ICommand
    {
        private readonly BagViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandOpenBag(BagViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.OnOpenFile();
        }
    }
}
