using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SanBag.Viewer.Views;

namespace SanBag.Viewer.ViewModels
{
    class MainViewModel
    {
        public UserControl CurrentView { get; set; }
        public BaseViewModel CurrentViewModel { get; set; }

        public MainViewModel()
        {
        }

        public void OpenFile(string resourcePath)
        {
            var fileName = Path.GetFileName(resourcePath);
            var fileInfo = LibSanBag.FileRecordInfo.Create(fileName);

            switch (fileInfo.Resource)
            {
                case LibSanBag.FileRecordInfo.ResourceType.TextureResource:
                    CurrentView = new TextureResourceView()
                    {
                        DataContext = new TextureResourceViewModel()
                        {
                            CurrentPath = resourcePath
                        }
                    };
                    break;
                default:
                    var view = new RawResourceView();
                    var viewModel = new RawResourceViewModel()
                    {
                        HexControl = view.HexEdit,
                        CurrentPath = resourcePath,
                    };
                    view.DataContext = viewModel;
                    CurrentView = view;
                    break;
            }

        }
    }
}
