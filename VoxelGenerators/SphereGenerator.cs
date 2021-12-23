using ModelGenerator;
using Newtonsoft.Json;
using VoxelCombinerGenerator;

namespace VoxelGenerators
{
    [VoxelGenerator("Sphere", typeof(SaveData))]
    public class SphereGenerator : IVoxelGenerator
    {
        public void CreatePanel(EditorContext context, out object generatorData)
        {
            var panel = new SphereGeneratorPanel();
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

        private static int[,,] GenerateVoxels(int radius, bool even, int material)
        {
            if(even)
            {
                int[,,] voxels = new int[radius * 2, radius * 2, radius * 2];
                FillVoxels(voxels, material, radius - 1, radius * radius - 1, radius, radius - 1);
                return voxels;
            }
            else
            {
                int[,,] voxels = new int[radius * 2 + 1, radius * 2 + 1, radius * 2 + 1];
                FillVoxels(voxels, material, radius, (radius + 1) * (radius + 1) - 1, radius, radius);
                return voxels;
            }
        }

        private static void FillVoxels(int[,,] voxels, int material, int size, int r, int c1, int c2)
        {
            for(int x = 0; x <= size; x++)
            {
                int xm = x * x;
                for(int y = 0; y <= size; y++)
                {
                    int ym = y * y;
                    for(int z = 0; z <= size; z++)
                    {
                        int m = ((xm + ym + z * z) <= r) ? material : -1;
                        voxels[x + c1, y + c1, z + c1] = m;
                        voxels[x + c1, c2 - y, z + c1] = m;
                        voxels[c2 - x, y + c1, z + c1] = m;
                        voxels[c2 - x, c2 - y, z + c1] = m;
                        voxels[x + c1, y + c1, c2 - z] = m;
                        voxels[x + c1, c2 - y, c2 - z] = m;
                        voxels[c2 - x, y + c1, c2 - z] = m;
                        voxels[c2 - x, c2 - y, c2 - z] = m;
                    }
                }
            }
        }

        [JsonObject]
        private class SaveData
        {
            [JsonProperty]
            public int radius = 1;
            [JsonProperty]
            public bool isEven = true;
        }

        private class GeneratorData
        {
            public SphereGeneratorPanel panel;

            public GeneratorData(SphereGeneratorPanel panel)
            {
                this.panel = panel;
            }

            public void SetSave(SaveData saveData)
            {
                panel.SetValues(saveData.radius, saveData.isEven);
            }

            public SaveData GetSave()
            {
                return new SaveData() {
                    radius = panel.GetRadius(),
                    isEven = panel.IsEven()
                };
            }

            public int[,,] Generate(int materialIndex)
            {
                return GenerateVoxels(panel.GetRadius(), panel.IsEven(), materialIndex);
            }
        }
    }
}