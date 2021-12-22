using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace VoxelGenerators
{
    public class BoxGeneratorPanel : Control
    {
        private Vec3iBox boxSize = null;

        private Vec3i size = new Vec3i(16, 16, 16);

        static BoxGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxGeneratorPanel), new FrameworkPropertyMetadata(typeof(BoxGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            boxSize = GetTemplateChild("boxSize") as Vec3iBox;

            boxSize.SetValue(size);
        }

        public Vec3i GetSize()
        {
            return boxSize == null ? size : boxSize.GetValue();
        }

        public void SetSize(Vec3i size)
        {
            if(boxSize != null)
            {
                boxSize.SetValue(size);
            }
            else
            {
                this.size = size.Clone();
            }
        }
    }
}