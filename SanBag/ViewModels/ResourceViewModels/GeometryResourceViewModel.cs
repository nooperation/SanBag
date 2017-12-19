using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSanBag.FileResources;

namespace SanBag.ViewModels.ResourceViewModels
{
    class GeometryResourceViewModel : BaseViewModel
    {
        protected override void LoadFromStream(Stream resourceStream)
        {
            var resource = new GeometryResource();
            resource.InitFromStream(resourceStream);
        }
    }
}
