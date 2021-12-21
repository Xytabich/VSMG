using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public class Vec3dBox : Vec3BoxBase<Vec3d>
    {
        static Vec3dBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Vec3dBox), new FrameworkPropertyMetadata(typeof(Vec3BoxBase)));
        }

        public override void SetValue(Vec3d value)
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

        protected override void InitInputs()
        {
            x.InitFloatField();
            y.InitFloatField();
            z.InitFloatField();
            x.SetDouble(value.X);
            y.SetDouble(value.Y);
            z.SetDouble(value.Z);
        }

        protected override void OnXChanged(object sender, TextChangedEventArgs e)
        {
            value.X = x.GetDouble(0);
        }

        protected override void OnYChanged(object sender, TextChangedEventArgs e)
        {
            value.Y = y.GetDouble(0);
        }

        protected override void OnZChanged(object sender, TextChangedEventArgs e)
        {
            value.Z = z.GetDouble(0);
        }
    }
}