using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public class Vec3dBox : Control
    {
        private TextBox x = null;
        private TextBox y = null;
        private TextBox z = null;
        private Vec3d value = new Vec3d(0, 0, 0);

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
            x.InitFloatField();
            y.InitFloatField();
            z.InitFloatField();
            x.SetDouble(value.X);
            y.SetDouble(value.Y);
            z.SetDouble(value.Z);
            x.TextChanged += OnXChanged;
            y.TextChanged += OnYChanged;
            z.TextChanged += OnZChanged;
        }

        public Vec3d GetValue()
        {
            return value;
        }

        public void SetValue(Vec3d value)
        {
            this.value.X = value.X;
            this.value.Y = value.Y;
            this.value.Z = value.Z;
            if(x != null)
            {
                x.SetDouble(value.X);
                y.SetDouble(value.Y);
                z.SetDouble(value.Z);
            }
        }

        private void OnXChanged(object sender, TextChangedEventArgs e)
        {
            value.X = x.GetDouble(0);
        }

        private void OnYChanged(object sender, TextChangedEventArgs e)
        {
            value.Y = y.GetDouble(0);
        }

        private void OnZChanged(object sender, TextChangedEventArgs e)
        {
            value.Z = z.GetDouble(0);
        }
    }
}