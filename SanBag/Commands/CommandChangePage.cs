﻿using System;
using System.Windows.Input;
using SanBag.ViewModels;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    public class CommandChangePage: ICommand
    {
        private readonly AtlasViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandChangePage(AtlasViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.ChangePage((int)parameter);
        }
    }
}
