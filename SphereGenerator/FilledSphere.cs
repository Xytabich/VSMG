using ModelGenerator;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;

namespace SphereGenerator
{
    [ShapeGenerator("Filled Sphere", typeof(PresetData))]
    public class FilledSphere : SphereGeneratorBase, IPresetShapeGenerator
    {
        public void ApplyPreset(object obj)
        {
            var preset = (PresetData)obj;
            panel.SetValues(preset.offset, preset.radius, preset.isEven);
        }

        public object CreatePreset()
        {
            return new PresetData() {
                offset = panel.GetOffset().Clone(),
                radius = panel.GetRadius(),
                isEven = panel.IsEven()
            };
        }

        protected override bool[,,] GenerateVoxels(int radius, bool even)
        {
            if(even)
            {
                bool[,,] voxels = new bool[radius * 2, radius * 2, radius * 2];
                FillVoxels(voxels, radius - 1, radius * radius - 1, radius, radius - 1);
                return voxels;
            }
            else
            {
                bool[,,] voxels = new bool[radius * 2 + 1, radius * 2 + 1, radius * 2 + 1];
                FillVoxels(voxels, radius, (radius + 1) * (radius + 1) - 1, radius, radius);
                return voxels;
            }
        }

        private void FillVoxels(bool[,,] voxels, int size, int r, int c1, int c2)
        {
            for(int x = 0; x <= size; x++)
            {
                int xm = x * x;
                for(int y = 0; y <= size; y++)
                {
                    int ym = y * y;
                    for(int z = 0; z <= size; z++)
                    {
                        if((xm + ym + z * z) <= r)
                        {
                            voxels[x + c1, y + c1, z + c1] = true;
                            voxels[x + c1, c2 - y, z + c1] = true;
                            voxels[c2 - x, y + c1, z + c1] = true;
                            voxels[c2 - x, c2 - y, z + c1] = true;
                            voxels[x + c1, y + c1, c2 - z] = true;
                            voxels[x + c1, c2 - y, c2 - z] = true;
                            voxels[c2 - x, y + c1, c2 - z] = true;
                            voxels[c2 - x, c2 - y, c2 - z] = true;
                        }
                    }
                }
            }
        }

        [JsonObject]
        public class PresetData
        {
            [JsonProperty]
            public int radius;

            [JsonProperty]
            public bool isEven;

            [JsonProperty]
            public Vec3d offset;
        }
    }
}