using SanBag.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SanBag.Commands
{
    public class CommandOpenBag : ICommand
    {
        public BagViewModel ViewModel { get; set; }

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandOpenBag(BagViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            ViewModel.OnOpenFile();
        }
    }
}
