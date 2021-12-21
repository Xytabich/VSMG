using ModelGenerator;
using System.Windows;
using System.Windows.Controls;

namespace VoxelGenerators
{
    public class SphereGeneratorPanel : Control
    {
        private TextBox sphereRadius = null;
        private CheckBox sphereEven = null;

        private int radius = 1;
        private bool isEven = true;

        static SphereGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SphereGeneratorPanel), new FrameworkPropertyMetadata(typeof(SphereGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            sphereRadius = GetTemplateChild("sphereRadius") as TextBox;
            sphereEven = GetTemplateChild("sphereEven") as CheckBox;

            sphereRadius.InitIntegerField();

            sphereRadius.SetInteger(radius);
            sphereEven.IsChecked = isEven;

            sphereRadius.TextChanged += OnRadiusChanged;
            sphereEven.Checked += OnEvenChanged;
            sphereEven.Unchecked += OnEvenChanged;
        }

        public int GetRadius()
        {
            return radius;
        }

        public bool IsEven()
        {
            return isEven;
        }

        public void SetValues(int radius, bool isEven)
        {
            this.radius = radius;
            this.isEven = isEven;
            if(sphereRadius != null)
            {
                sphereRadius.SetInteger(radius);
                sphereEven.IsChecked = isEven;
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