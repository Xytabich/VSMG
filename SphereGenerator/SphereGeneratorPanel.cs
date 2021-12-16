using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace SphereGenerator
{
    public class SphereGeneratorPanel : Control
    {
        private TextBox sphereRadius = null;
        private CheckBox sphereEven = null;
        private Vec3dBox sphereOffset = null;

        private int radius = 1;
        private bool isEven = true;
        private Vec3d offset = Vec3d.Zero;

        static SphereGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SphereGeneratorPanel), new FrameworkPropertyMetadata(typeof(SphereGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            sphereRadius = GetTemplateChild("sphereRadius") as TextBox;
            sphereEven = GetTemplateChild("sphereEven") as CheckBox;
            sphereOffset = GetTemplateChild("sphereOffset") as Vec3dBox;

            sphereRadius.InitIntegerField();

            sphereRadius.SetInteger(radius);
            sphereEven.IsChecked = isEven;
            sphereOffset.SetValue(offset);

            sphereRadius.TextChanged += OnRadiusChanged;
            sphereEven.Checked += OnEvenChanged;
            sphereEven.Unchecked += OnEvenChanged;
        }

        public Vec3d GetOffset()
        {
            return sphereOffset == null ? offset : sphereOffset.GetValue();
        }

        public int GetRadius()
        {
            return radius;
        }

        public bool IsEven()
        {
            return isEven;
        }

        public void SetValues(Vec3d offset, int radius, bool isEven)
        {
            this.radius = radius;
            this.isEven = isEven;
            this.offset = offset.Clone();
            if(sphereRadius != null)
            {
                sphereRadius.SetInteger(radius);
                sphereEven.IsChecked = isEven;
                sphereOffset.SetValue(offset);
            }
        }

        private void OnRadiusChanged(object sender, TextChangedEventArgs e)
        {
            radius = sphereRadius.GetInteger(1);
        }

        private void OnEvenChanged(object sender, RoutedEventArgs e)
        {
            isEven = sphereEven.IsChecked == true;
        }
    }
}