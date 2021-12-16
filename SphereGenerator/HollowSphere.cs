using ModelGenerator;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;

namespace SphereGenerator
{
    [ShapeGenerator("Hollow Sphere", typeof(PresetData))]
    public class HollowSphere : SphereGeneratorBase, IPresetShapeGenerator
    {
        private HollowSphereGeneratorPanel hollowPanel = null;

        public override void ShowPanel(EditorContext context)
        {
            if(hollowPanel == null)
            {
                panel = hollowPanel = new HollowSphereGeneratorPanel();
            }
            context.parent.Children.Add(panel);
        }

        public void ApplyPreset(object obj)
        {
            var preset = (PresetData)obj;
            hollowPanel.SetValues(preset.offset, preset.outerRadius, preset.innerRadius, preset.isEven);
        }

        public object CreatePreset()
        {
            return new PresetData() {
                offset = hollowPanel.GetOffset(),
                outerRadius = hollowPanel.GetRadius(),
                innerRadius = hollowPanel.GetInnerRadius(),
                isEven = hollowPanel.IsEven()
            };
        }

        protected override bool[,,] GenerateVoxels(int radius, bool even)
        {
            int innerRadius = hollowPanel.GetInnerRadius();
            if(even)
            {
                bool[,,] voxels = new bool[radius * 2, radius * 2, radius * 2];
                FillVoxels(voxels, radius - 1, radius * radius - 1, innerRadius * innerRadius - 1, radius, radius - 1);
                return voxels;
            }
            else
            {
                bool[,,] voxels = new bool[radius * 2 + 1, radius * 2 + 1, radius * 2 + 1];
                FillVoxels(voxels, radius, (radius + 1) * (radius + 1) - 1, (innerRadius + 1) * (innerRadius + 1) - 1, radius, radius);
                return voxels;
            }
        }

        private void FillVoxels(bool[,,] voxels, int size, int or, int ir, int c1, int c2)
        {
            for(int x = 0; x <= size; x++)
            {
                int xm = x * x;
                for(int y = 0; y <= size; y++)
                {
                    int ym = y * y;
                    for(int z = 0; z <= size; z++)
                    {
                        int r = xm + ym + z * z;
                        if(r <= or && r > ir)
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
            public int outerRadius;

            [JsonProperty]
            public int innerRadius;

            [JsonProperty]
            public bool isEven;

            [JsonProperty]
            public Vec3d offset;
        }
    }
}