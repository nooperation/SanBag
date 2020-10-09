using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSanBag.FileResources;
using Newtonsoft.Json;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class AudioGraphResourceViewModel : BaseViewModel
    {
        private string _filename;
        public string Filename
        {
            get => _filename;
            set
            {
                if (value == _filename)
                {
                    return;
                }
                _filename = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                {
                    return;
                }
                _text = value;
                OnPropertyChanged();
            }
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var resource = AudioGraphResource.Create(version);
            resource.InitFromStream(resourceStream);

            Filename = "N/A";
            Text = JsonConvert.SerializeObject(resource, Formatting.Indented);
        }
    }
}
