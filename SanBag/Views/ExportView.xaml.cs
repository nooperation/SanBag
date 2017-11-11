using LibSanBag;
using SanBag.Commands;
using SanBag.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SanBag.Views
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportView : Window
    {
        public ExportView()
        {
            InitializeComponent();
        }

        private async void ExportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ExportViewModel;
            await viewModel.StartAsync();
        }
        
        private void ExportWindow_Closing(object sender, CancelEventArgs e)
        {
            var viewModel = this.DataContext as ExportViewModel;
            viewModel.CancelExport();
        }
    }
}
