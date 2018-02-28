using System;
using System.Windows.Input;
using AtlasView.ViewModels;

namespace AtlasView.Commands
{
    public class CommandNextPage: ICommand
    {
        private readonly AtlasViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandNextPage(AtlasViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.NextPage();
        }
    }
}
