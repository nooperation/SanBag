using System;
using System.Windows.Input;
using CommonUI.ViewModels.ResourceViewModels;

namespace CommonUI.Commands
{
    public class CommandPlaySound : ICommand
    {
        private readonly SoundResourceViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandPlaySound(SoundResourceViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.PlaySound();
        }
    }
}
