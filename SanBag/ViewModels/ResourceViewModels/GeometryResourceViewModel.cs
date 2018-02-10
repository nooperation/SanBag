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

namespace SanBag.ViewModels.ResourceViewModels
{
    class GeometryResourceViewModel : BaseViewModel
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

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var resource = GeometryResource.Create(version);
            resource.InitFromStream(resourceStream);

            var positions = new List<Point3D>();
            for (int i = 0; i < resource.Vertices.Count; i += 3)
            {
                positions.Add(new Point3D(resource.Vertices[i], resource.Vertices[i + 1], resource.Vertices[i + 2]));
            }

            var indices = resource.Indices.Select(n => (int) n);
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
