using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SanBag.Viewer.ViewModels;

namespace SanBag.Viewer.Commands
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
