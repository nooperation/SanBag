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
                    CurrentViewModel = new TextureResourceViewModel()
                    {
                        CurrentPath = resourcePath
                    };
                    CurrentView = new TextureResourceView()
                    {
                        DataContext = CurrentViewModel
                    };
                    break;
            }

        }
    }
}
