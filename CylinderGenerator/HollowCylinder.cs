using ModelGenerator;
using System;
using System.Windows.Controls;

namespace CylinderGenerator
{
    [ShapeGenerator("Hollow Cylinder")]
    public class HollowCylinder : CylinderGeneratorBase
    {
        private HollowCylinderGeneratorPanel hollowPanel;

        public override void ShowPanel(Panel parent)
        {
            parent.Children.Add(panel = hollowPanel = new HollowCylinderGeneratorPanel());
        }

        public override void OnHide()
        {
            base.OnHide();
            hollowPanel = null;
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
            int innerRadius = hollowPanel.GetInnerRadius();
            if(even)
            {
                FillVoxels(voxelSetter, radius - 1, radius * radius - 1, innerRadius * innerRadius - 1, radius, radius - 1);
            }
            else
            {
                FillVoxels(voxelSetter, radius, (radius + 1) * (radius + 1) - 1, (innerRadius + 1) * (innerRadius + 1) - 1, radius, radius);
            }
            return voxels;
        }

        private void FillVoxels(Action<int, int> voxelSetter, int size, int or, int ir, int c1, int c2)
        {
            for(int x = 0; x <= size; x++)
            {
                int xm = x * x;
                for(int y = 0; y <= size; y++)
                {
                    int r = xm + y * y;
                    if(r <= or && r > ir)
                    {
                        voxelSetter(x + c1, y + c1);
                        voxelSetter(x + c1, c2 - y);
                        voxelSetter(c2 - x, y + c1);
                        voxelSetter(c2 - x, c2 - y);
                    }
                }
            }
        }
    }
}