using System.IO;
using System.Text;
using CommonUI.Annotations;
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

        private ScriptMetadataResource.ScriptMetadata _currentScript = new ScriptMetadataResource.ScriptMetadata();
        public ScriptMetadataResource.ScriptMetadata CurrentScript
        {
            get
            {
                return _currentScript;
            }
            set
            {
                _currentScript = value;
                DumpScriptMetadata();
                OnPropertyChanged();
            }
        }

        private string _currentScriptString;
        public string CurrentScriptString
        {
            get => _currentScriptString;
            set
            {
                _currentScriptString = value;
                OnPropertyChanged();
            }
        }

        private void DumpScriptMetadata()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{CurrentScript.ClassName} ({CurrentScript.DisplayName})");
            if (CurrentScript.Tooltip?.Length > 0)
            {
                sb.AppendLine($"{CurrentScript.Tooltip}");
            }
            sb.AppendLine();

            if(CurrentScript.Properties.Count > 0)
            {
                foreach (var property in CurrentScript.Properties)
                {
                    sb.AppendLine($"    {property.Name} ({property.Type})");
                    if (property.Attributes.Count > 0)
                    {
                        foreach (var attribute in property.Attributes)
                        {
                            var padding = new string(' ', 8 + attribute.Key.Length + 3);
                            var attributeValue = attribute.Value.Replace("\n", "\n" + padding);

                            sb.AppendLine($"        {attribute.Key} = {attributeValue}");
                        }
                    }
                    sb.AppendLine();
                }
            }

            CurrentScriptString = sb.ToString();
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var tempResource = ScriptMetadataResource.Create(version);
            tempResource.InitFromStream(resourceStream);

            Resource = tempResource;
        }
    }
}
