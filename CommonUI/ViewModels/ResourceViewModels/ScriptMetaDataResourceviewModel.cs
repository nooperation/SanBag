using System.IO;
using LibSanBag.FileResources;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class ScriptMetadataResourceViewModel : BaseViewModel
    {
        private ScriptMetadataResource _resource;
        public ScriptMetadataResource Resource
        {
            get => _resource;
            set
            {
                if (value == _resource)
                {
                    return;
                }
                _resource = value;
                OnPropertyChanged();
            }
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var tempResource = ScriptMetadataResource.Create(version);
            tempResource.InitFromStream(resourceStream);

            Resource = tempResource;
        }
    }
}
