using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SanBag.ViewModels.ResourceViewModels;

namespace SanBag.Commands
{
    class CommandPlaySound : ICommand
    {
        private SoundResourceViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandPlaySound(SoundResourceViewModel viewModel)
        {
            this._viewModel = viewModel;
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
