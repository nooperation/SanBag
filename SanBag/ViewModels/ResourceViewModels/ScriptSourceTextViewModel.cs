using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanBag.ViewModels.ResourceViewModels
{
    class ScriptSourceTextViewModel : BaseViewModel
    {
        public string FileName { get; set; }
        public string SourceCode { get; set; }

        protected override void LoadFromStream(Stream resourceStream)
        {
            throw new NotImplementedException();
        }
    }
}
