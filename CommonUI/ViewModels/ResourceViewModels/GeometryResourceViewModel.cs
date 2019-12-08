using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using LibSanBag.FileResources;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class GeometryResourceViewModel : BaseViewModel
    {
        private Model3D _model;
        public Model3D Model
        {
            get => _model;
            set
            {
                if (Equals(value, _model))
                {
                    return;
                }

                _model = value;
                OnPropertyChanged();
            }
        }

        private List<float> GetVerticies(GeometryResource resource)
        {
            var geometryVertexStream = resource.Resource.GeometryData.VertexStreams.Find(n => n.Type.Name == "position");
            var rawVertexData = geometryVertexStream.Data;
            var verticies = new List<float>();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(rawVertexData)))
            {
                for (int i = 0; i < rawVertexData.Length; i += 4)
                {
                    var vert = reader.ReadSingle();
                    verticies.Add(vert);
                }
            }

            return verticies;
        }

        private List<int> GetIndices(GeometryResource resource)
        {
            var rawIndexData = resource.Resource.GeometryData.IndexData; ;
            var indices = new List<int>();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(rawIndexData)))
            {
                if(resource.Resource.GeometryData.IndexFormat == 4)
                {
                    for (int i = 0; i < rawIndexData.Length; i += 4)
                    {
                        var index = reader.ReadInt32();
                        indices.Add(index);
                    }
                }
                else
                {
                    for (int i = 0; i < rawIndexData.Length; i += 2)
                    {
                        var index = reader.ReadInt16();
                        indices.Add(index);
                    }
                }
            }

            return indices;
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var resource = GeometryResource.Create(version);
            resource.InitFromStream(resourceStream);

            var vertices = GetVerticies(resource);
            var indices = GetIndices(resource);

            var positions = new List<Point3D>();
            for (int i = 0; i < vertices.Count; i += 3)
            {
                positions.Add(new Point3D(vertices[i], vertices[i + 1], vertices[i + 2]));
            }

            var mesh = new Mesh3D(positions, indices);
            var geometry = mesh.ToMeshGeometry3D();

            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
            var insideMaterial = MaterialHelper.CreateMaterial(Colors.Yellow);

            Model = new GeometryModel3D
            {
                Geometry = geometry,
                Material = greenMaterial,
                BackMaterial = insideMaterial
            };
        }
    }
}
