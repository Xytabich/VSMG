using ModelGenerator;
using Newtonsoft.Json;
using System;
using Vintagestory.API.MathTools;

namespace CylinderGenerator
{
    [ShapeGenerator("Filled Cylinder", typeof(PresetData))]
    public class FilledCylinder : CylinderGeneratorBase, IPresetShapeGenerator
    {
        public void ApplyPreset(object obj)
        {
            var preset = (PresetData)obj;
            panel.SetValues(preset.offset, preset.radius, preset.length, preset.axis, preset.isEven);
        }

        public object CreatePreset()
        {
            return new PresetData() {
                offset = panel.GetOffset(),
                radius = panel.GetRadius(),
                length = panel.GetLength(),
                axis = panel.GetAxis(),
                isEven = panel.IsEven()
            };
        }

        protected override bool[,,] GenerateVoxels(int radius, int length, int axis, bool even)
        {
            Action<int, int> voxelSetter = null;
            bool[,,] voxels = null;
            int circleSize = radius * 2 + (even ? 0 : 1);
            switch(axis)
            {
                case 0:
                    voxels = new bool[length, circleSize, circleSize];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[k, i, j] = true;
                        }
                    };
                    break;
                case 1:
                    voxels = new bool[circleSize, length, circleSize];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[i, k, j] = true;
                        }
                    };
                    break;
                case 2:
                    voxels = new bool[circleSize, circleSize, length];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[i, j, k] = true;
                        }
                    };
                    break;
            }
            if(even)
            {
                FillVoxels(voxelSetter, radius - 1, radius * radius - 1, radius, radius - 1);
            }
            else
            {
                FillVoxels(voxelSetter, radius, (radius + 1) * (radius + 1) - 1, radius, radius);
            }
            return voxels;
        }

        private void FillVoxels(Action<int, int> voxelSetter, int size, int r, int c1, int c2)
        {
            for(int x = 0; x <= size; x++)
            {
                int xm = x * x;
                for(int y = 0; y <= size; y++)
                {
                    if((xm + y * y) <= r)
                    {
                        voxelSetter(x + c1, y + c1);
                        voxelSetter(x + c1, c2 - y);
                        voxelSetter(c2 - x, y + c1);
                        voxelSetter(c2 - x, c2 - y);
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
            public int length;

            [JsonProperty]
            public int axis;

            [JsonProperty]
            public bool isEven;

            [JsonProperty]
            public Vec3d offset;
        }
    }
}