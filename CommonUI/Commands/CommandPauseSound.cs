using System;
using System.Windows.Input;
using CommonUI.ViewModels.ResourceViewModels;

namespace CommonUI.Commands
{
    public class CommandPauseSound : ICommand
    {
        private readonly SoundResourceViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandPauseSound(SoundResourceViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.PauseSound();
        }
    }
}
