using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSanBag.FileResources;

namespace SanBag.ViewModels.ResourceViewModels
{
    class ScriptSourceTextViewModel : BaseViewModel
    {
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                if (value == _fileName)
                {
                    return;
                }
                _fileName = value;
                OnPropertyChanged();
            }
        }

        private string _sourceCode;
        public string SourceCode
        {
            get => _sourceCode;
            set
            {
                if (value == _sourceCode)
                {
                    return;
                }
                _sourceCode = value;
                OnPropertyChanged();
            }
        }

        protected override void LoadFromStream(Stream resourceStream)
        {
            var resource = new ScriptSourceTextResource();
            resource.InitFromStream(resourceStream);

            FileName = resource.Filename;
            SourceCode = resource.Source;
        }
    }
}
