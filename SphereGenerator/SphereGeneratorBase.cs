using ModelGenerator;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace SphereGenerator
{
    public abstract class SphereGeneratorBase : IShapeGenerator
    {
        protected SphereGeneratorPanel panel = null;

        public virtual void ShowPanel(EditorContext context)
        {
            if(panel == null) panel = new SphereGeneratorPanel();
            context.parent.Children.Add(panel);
        }

        public virtual void OnHide() { }

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
            var material = context.materials.Count > 0 ? context.materials[0] : null;

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
                            if(material != null)
                            {
                                material.ApplyAllProperties(element);
                            }
                            else
                            {
                                foreach(var face in element.Faces)
                                {
                                    face.Value.Texture = "#null";
                                }
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