using System.ComponentModel;
using System.Windows;
using SanBag.ViewModels;

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
