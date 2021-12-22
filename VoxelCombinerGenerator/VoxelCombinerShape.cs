using ModelGenerator;
using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace VoxelCombinerGenerator
{
    [ShapeGenerator("Voxel Combiner", typeof(PresetData))]
    public class VoxelCombinerShape : IShapeGenerator, IPresetShapeGenerator
    {
        private VoxelCombinerShapePanel panel = null;
        private object instance;

        public void ApplyPreset(object obj)
        {
            var preset = (PresetData)obj;
            panel.SetOffset(preset.offset);
            VoxelCombiner.ApplySaveData(instance, preset.data);
        }

        public object CreatePreset()
        {
            return new PresetData() {
                offset = panel.GetOffset().Clone(),
                data = (VoxelCombiner.GeneratorSave)VoxelCombiner.CreateSaveData(instance)
            };
        }

        public void ShowPanel(EditorContext context)
        {
            if(panel == null)
            {
                panel = new VoxelCombinerShapePanel();
                context.parent.Children.Add(panel);
                panel.ApplyTemplate();

                context.parent = panel.GetGeneratorAnchor();
                VoxelCombiner.CreatePanel(context, out instance);
            }
            else
            {
                context.parent.Children.Add(panel);
            }
        }

        public void OnHide() { }

        public void Generate(GeneratorContext context)
        {
            var voxels = VoxelCombiner.Generate(new VoxelGeneratorContext() { materials = context.materials, generatorData = instance });

            var offset = panel.GetOffset();
            int sizeX = voxels.GetLength(0);
            int sizeY = voxels.GetLength(1);
            int sizeZ = voxels.GetLength(2);
            var cuboids = VoxelModelUtils.VoxelsToCuboids(voxels, context.materials, true, sizeX, sizeY, sizeZ);
            VoxelModelUtils.MergeNeighbors(cuboids, sizeX, sizeY, sizeZ);
            ModelUtils.RemoveInvisibleCuboids(cuboids, sizeX, sizeY, sizeZ);

            var shape = context.shape;
            var exported = new HashSet<CuboidInfo>();
            var elements = new List<ShapeElement>();

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
                            var material = context.materials.Count > voxel.material ? context.materials[voxel.material] : null;
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

        [JsonObject]
        private class PresetData
        {
            [JsonProperty]
            public Vec3d offset;
            [JsonProperty]
            public VoxelCombiner.GeneratorSave data;
        }
    }
}
