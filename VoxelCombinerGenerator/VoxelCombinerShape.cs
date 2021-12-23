using ModelGenerator;
using Newtonsoft.Json;
using System;
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
            var volume = VoxelCombiner.Generate(new VoxelGeneratorContext() { materials = context.materials, generatorData = instance });

            var offset = panel.GetOffset().Clone();
            offset.X += volume.x;
            offset.Y += volume.y;
            offset.Z += volume.z;
            int sizeX = volume.sizeX;
            int sizeY = volume.sizeY;
            int sizeZ = volume.sizeZ;
            var cuboids = VoxelModelUtils.VoxelsToCuboids(volume.voxels, context.materials, true, sizeX, sizeY, sizeZ);
            VoxelModelUtils.MergeNeighbors(cuboids, Math.Min(context.shape.TextureWidth, context.shape.TextureHeight), sizeX, sizeY, sizeZ);
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
