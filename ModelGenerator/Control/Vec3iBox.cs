using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public class Vec3iBox : Vec3BoxBase<Vec3i>
    {
        static Vec3iBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Vec3iBox), new FrameworkPropertyMetadata(typeof(Vec3BoxBase)));
        }

        public override void SetValue(Vec3i value)
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
            x.InitIntegerField();
            y.InitIntegerField();
            z.InitIntegerField();
            x.SetInteger(value.X);
            y.SetInteger(value.Y);
            z.SetInteger(value.Z);
        }

        protected override void OnXChanged(object sender, TextChangedEventArgs e)
        {
            value.X = x.GetInteger(0);
        }

        protected override void OnYChanged(object sender, TextChangedEventArgs e)
        {
            value.Y = y.GetInteger(0);
        }

        protected override void OnZChanged(object sender, TextChangedEventArgs e)
        {
            value.Z = z.GetInteger(0);
        }
    }
}