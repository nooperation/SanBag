using System;
using System.Windows.Input;
using CommonUI.ViewModels.ResourceViewModels;

namespace CommonUI.Commands
{
    public class CommandDisassembleDll : ICommand
    {
        private readonly IDisassemblable _disassemblable;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandDisassembleDll(IDisassemblable disassemblable)
        {
            _disassemblable = disassemblable;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _disassemblable.Disassemble();
        }
    }
}
