using System;
using System.Windows.Input;
using SanBag.ViewModels.Standalone;

namespace SanBag.Commands
{
    class CommandSaveAs : ICommand
    {
        private readonly ISavable savable;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandSaveAs(ISavable savable)
        {
            this.savable = savable;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            savable.SaveAs();
        }
    }
}
