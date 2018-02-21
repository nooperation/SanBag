using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LibSanBag;
using SanBag.ViewModels;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    class CommandSearch : ICommand
    {
        private readonly AtlasViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandSearch(AtlasViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            var query = parameter as string;

            _viewModel.Search(query);
        }
    }
}
