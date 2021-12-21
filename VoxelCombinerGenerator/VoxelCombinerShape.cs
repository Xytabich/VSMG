using ModelGenerator;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                offset = panel.GetOffset(),
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
