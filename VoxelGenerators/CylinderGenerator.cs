using ModelGenerator;
using Newtonsoft.Json;
using System;
using VoxelCombinerGenerator;

namespace VoxelGenerators
{
    [VoxelGenerator("Cylinder", typeof(SaveData))]
    public class CylinderGenerator : IVoxelGenerator
    {
        public void CreatePanel(EditorContext context, out object generatorData)
        {
            var panel = new CylinderGeneratorPanel();
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

        public int[,,] Generate(VoxelGeneratorContext context)
        {
            return ((GeneratorData)context.generatorData).Generate(context.materialIndex);
        }

        private static int[,,] GenerateVoxels(int radius, int length, int axis, bool even, int material)
        {
            Action<int, int> voxelSetter = null;
            int[,,] voxels = null;
            int circleSize = radius * 2 + (even ? 0 : 1);
            switch(axis)
            {
                case 0:
                    voxels = new int[length, circleSize, circleSize];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[k, i, j] = material;
                        }
                    };
                    break;
                case 1:
                    voxels = new int[circleSize, length, circleSize];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[i, k, j] = material;
                        }
                    };
                    break;
                case 2:
                    voxels = new int[circleSize, circleSize, length];
                    voxelSetter = (i, j) => {
                        for(int k = 0; k < length; k++)
                        {
                            voxels[i, j, k] = material;
                        }
                    };
                    break;
            }
            unsafe
            {
                int count = circleSize * circleSize * length;
                unsafe
                {
                    fixed(int* ptr = voxels)
                    {
                        for(int i = 0; i < count; i++)
                        {
                            ptr[i] = -1;
                        }
                    }
                }
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

        private static void FillVoxels(Action<int, int> voxelSetter, int size, int r, int c1, int c2)
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
        private class SaveData
        {
            [JsonProperty]
            public int radius = 1;
            [JsonProperty]
            public int length = 1;
            [JsonProperty]
            public int axis = 0;
            [JsonProperty]
            public bool isEven = true;
        }

        private class GeneratorData
        {
            public CylinderGeneratorPanel panel;

            public GeneratorData(CylinderGeneratorPanel panel)
            {
                this.panel = panel;
            }

            public void SetSave(SaveData saveData)
            {
                panel.SetValues(saveData.radius, saveData.length, saveData.axis, saveData.isEven);
            }

            public SaveData GetSave()
            {
                return new SaveData() {
                    radius = panel.GetRadius(),
                    length = panel.GetLength(),
                    axis = panel.GetAxis(),
                    isEven = panel.IsEven()
                };
            }

            public int[,,] Generate(int materialIndex)
            {
                return GenerateVoxels(panel.GetRadius(), panel.GetLength(), panel.GetAxis(), panel.IsEven(), materialIndex);
            }
        }
    }
}