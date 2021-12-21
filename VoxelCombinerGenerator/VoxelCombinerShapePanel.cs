using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace VoxelCombinerGenerator
{
    public class VoxelCombinerShapePanel : Control
    {
        private Vec3dBox shapeOffset = null;
        private Panel generatorAnchor = null;

        static VoxelCombinerShapePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoxelCombinerShapePanel), new FrameworkPropertyMetadata(typeof(VoxelCombinerShapePanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            shapeOffset = GetTemplateChild("shapeOffset") as Vec3dBox;
            generatorAnchor = GetTemplateChild("generatorAnchor") as Panel;
        }

        public Vec3d GetOffset()
        {
            return shapeOffset.GetValue();
        }

        public void SetOffset(Vec3d offset)
        {
            shapeOffset.SetValue(offset);
        }

        public Panel GetGeneratorAnchor()
        {
            return generatorAnchor;
        }
    }
}