﻿using System;
using System.Windows.Input;
using SanBag.ViewModels;
using SanBag.ViewModels.BagViewModels;

namespace SanBag.Commands
{
    public class CommandCancelExport : ICommand
    {
        private readonly ExportViewModel _viewModel;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CommandCancelExport(ExportViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _viewModel.CancelExport();
        }
    }
}
