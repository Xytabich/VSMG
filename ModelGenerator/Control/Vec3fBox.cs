using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public class Vec3fBox : Vec3BoxBase<Vec3f>
    {
        static Vec3fBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Vec3fBox), new FrameworkPropertyMetadata(typeof(Vec3BoxBase)));
        }

        public override void SetValue(Vec3f value)
        {
            this.value.X = value.X;
            this.value.Y = value.Y;
            this.value.Z = value.Z;
            if(x != null)
            {
                x.SetSingle(value.X);
                y.SetSingle(value.Y);
                z.SetSingle(value.Z);
            }
        }

        protected override void InitInputs()
        {
            x.InitFloatField();
            y.InitFloatField();
            z.InitFloatField();
            x.SetSingle(value.X);
            y.SetSingle(value.Y);
            z.SetSingle(value.Z);
        }

        protected override void OnXChanged(object sender, TextChangedEventArgs e)
        {
            value.X = x.GetSingle(0);
        }

        protected override void OnYChanged(object sender, TextChangedEventArgs e)
        {
            value.Y = y.GetSingle(0);
        }

        protected override void OnZChanged(object sender, TextChangedEventArgs e)
        {
            value.Z = z.GetSingle(0);
        }
    }
}