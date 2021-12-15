using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public class Vec3dBox : Control
    {
        private TextBox x, y, z;

        static Vec3dBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Vec3dBox), new FrameworkPropertyMetadata(typeof(Vec3dBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            x = GetTemplateChild("x") as TextBox;
            y = GetTemplateChild("y") as TextBox;
            z = GetTemplateChild("z") as TextBox;
            ControlUtils.InitFloatField(x);
            ControlUtils.InitFloatField(y);
            ControlUtils.InitFloatField(z);
        }

        public Vec3d GetValue()
        {
            return new Vec3d(x.GetDouble(0), y.GetDouble(0), z.GetDouble(0));
        }
    }
}