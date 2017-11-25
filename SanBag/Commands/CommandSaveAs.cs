using System;
using System.Windows.Input;
using SanBag.ViewModels.ResourceViewModels;

namespace SanBag.Commands
{
    class CommandSaveAs : ICommand
    {
        private readonly ISavable _savable;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandSaveAs(ISavable savable)
        {
            _savable = savable;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _savable.SaveAs();
        }
    }
}
