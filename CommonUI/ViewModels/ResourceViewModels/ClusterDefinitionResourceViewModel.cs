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
    public class ClusterDefinitionResourceViewModel : BaseViewModel
    {
        private string _geometryId;
        public string GeometryId
        {
            get => _geometryId;
            set
            {
                if (value == _geometryId)
                {
                    return;
                }
                _geometryId = value;
                OnPropertyChanged();
            }
        }

        private string _shapeId;
        public string ShapeId
        {
            get => _shapeId;
            set
            {
                if (value == _shapeId)
                {
                    return;
                }
                _shapeId = value;
                OnPropertyChanged();
            }
        }


        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                {
                    return;
                }
                _name = value;
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
            var resource = ClusterDefinitionResource.Create(version);
            resource.InitFromStream(resourceStream);

            GeometryId = "";
            ShapeId = "";

            if(resource.Resource.ObjectsDefs.Count > 0)
            {
                var objectDef = resource.Resource.ObjectsDefs[0];

                if (objectDef.EntityInfos.Count > 0)
                {
                    var entityInfo = objectDef.EntityInfos[0];

                    ShapeId = entityInfo.RigidBodyDef?.Shape ?? "";
                    GeometryId = entityInfo.StaticMeshComponentDef?.MeshComponent_V3?.ModelDefinition?.GeometryUuid ?? "";
                }

                Name = objectDef.Name ?? "";
            }

            Text = JsonConvert.SerializeObject(resource, Formatting.Indented);
        }
    }
}
