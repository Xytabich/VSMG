using ModelGenerator;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace SphereGenerator
{
    public abstract class SphereGeneratorBase : IShapeGenerator
    {
        protected SphereGeneratorPanel panel;

        public virtual void ShowPanel(EditorContext context)
        {
            context.parent.Children.Add(panel = new SphereGeneratorPanel());
        }

        public virtual void OnHide()
        {
            panel = null;
        }

        public void Generate(GeneratorContext context)
        {
            var offset = panel.GetOffset();
            var voxels = GenerateVoxels(panel.GetRadius(), panel.IsEven());
            int size = voxels.GetLength(0);
            var cuboids = ModelUtils.VoxelsToCuboids(voxels, size, size, size);
            ModelUtils.MergeNeighbors(cuboids, size, size, size);
            ModelUtils.RemoveInvisibleCuboids(cuboids, size, size, size);

            var shape = context.shape;
            var exported = new HashSet<CuboidInfo>();
            var elements = new List<ShapeElement>();
            string material = shape.Textures.Count > 0 ? ("#" + shape.Textures.First().Key) : "#null";

            int counter = 0;
            for(int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    for(int z = 0; z < size; z++)
                    {
                        var voxel = cuboids[x, y, z];
                        if(voxel != null && exported.Add(voxel))
                        {
                            var element = ModelUtils.CuboidToShapeElement(voxel, offset, shape.TextureWidth, shape.TextureHeight);
                            element.Name = "box" + (counter++);
                            foreach(var face in element.Faces)
                            {
                                face.Value.Texture = material;
                            }
                            elements.Add(element);
                        }
                    }
                }
            }
            shape.Elements = elements.ToArray();
        }

        protected abstract bool[,,] GenerateVoxels(int radius, bool even);
    }
}