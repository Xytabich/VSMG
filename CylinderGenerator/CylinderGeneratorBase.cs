using ModelGenerator;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace CylinderGenerator
{
    public abstract class CylinderGeneratorBase : IShapeGenerator
    {
        protected CylinderGeneratorPanel panel;

        public virtual void ShowPanel(EditorContext context)
        {
            context.parent.Children.Add(panel = new CylinderGeneratorPanel());
        }

        public virtual void OnHide()
        {
            panel = null;
        }

        public void Generate(GeneratorContext context)
        {
            var offset = panel.GetOffset();
            var voxels = GenerateVoxels(panel.GetRadius(), panel.GetLength(), panel.GetAxis(), panel.IsEven());
            int sizeX = voxels.GetLength(0);
            int sizeY = voxels.GetLength(1);
            int sizeZ = voxels.GetLength(2);
            var cuboids = ModelUtils.VoxelsToCuboids(voxels, sizeX, sizeY, sizeZ);
            ModelUtils.MergeNeighbors(cuboids, sizeX, sizeY, sizeZ);
            ModelUtils.RemoveInvisibleCuboids(cuboids, sizeX, sizeY, sizeZ);

            var shape = context.shape;
            var exported = new HashSet<CuboidInfo>();
            var elements = new List<ShapeElement>();
            var material = context.materials.Count > 0 ? context.materials[0] : null;

            int counter = 0;
            for(int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    for(int z = 0; z < sizeZ; z++)
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

        protected abstract bool[,,] GenerateVoxels(int radius, int length, int axis, bool even);
    }
}