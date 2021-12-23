using ModelGenerator;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;
using VoxelCombinerGenerator;

namespace VoxelGenerators
{
    [VoxelGenerator("Box", typeof(SaveData))]
    public class BoxGenerator : IVoxelGenerator
    {
        public void CreatePanel(EditorContext context, out object generatorData)
        {
            var panel = new BoxGeneratorPanel();
            context.parent.Children.Add(panel);
            generatorData = new GeneratorData(panel);
        }

        public void OnPanelDestroyed(object generatorData) { }

        public void ApplySaveData(object generatorData, object saveData)
        {
            ((GeneratorData)generatorData).SetSave((SaveData)saveData);
        }

        public object CreateSaveData(object generatorData)
        {
            return ((GeneratorData)generatorData).GetSave();
        }

        public VoxelVolume Generate(VoxelGeneratorContext context)
        {
            return new VoxelVolume(0, 0, 0, ((GeneratorData)context.generatorData).Generate(context.materialIndex));
        }

        private static unsafe int[,,] GenerateVoxels(int x, int y, int z, int material)
        {
            int length = x * y * z;
            int[,,] voxels = new int[x, y, z];
            unsafe
            {
                fixed(int* ptr = voxels)
                {
                    for(int i = 0; i < length; i++)
                    {
                        ptr[i] = material;
                    }
                }
            }
            return voxels;
        }

        [JsonObject]
        private class SaveData
        {
            [JsonProperty]
            public Vec3i size = new Vec3i(16, 16, 16);
        }

        private class GeneratorData
        {
            public BoxGeneratorPanel panel;

            public GeneratorData(BoxGeneratorPanel panel)
            {
                this.panel = panel;
            }

            public void SetSave(SaveData saveData)
            {
                panel.SetSize(saveData.size);
            }

            public SaveData GetSave()
            {
                return new SaveData() {
                    size = panel.GetSize().Clone()
                };
            }

            public int[,,] Generate(int materialIndex)
            {
                var size = panel.GetSize();
                return GenerateVoxels(size.X, size.Y, size.Z, materialIndex);
            }
        }
    }
}