using ModelGenerator;
using System.Windows;
using System.Windows.Controls;

namespace VoxelGenerators
{
    public class CylinderGeneratorPanel : Control
    {
        private TextBox cylinderRadius;
        private TextBox cylinderLength;
        private ComboBox cylinderAxis;
        private CheckBox cylinderEven;

        private int radius = 1;
        private int length = 1;
        private int axis = 0;
        private bool isEven = true;

        static CylinderGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CylinderGeneratorPanel), new FrameworkPropertyMetadata(typeof(CylinderGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            cylinderRadius = GetTemplateChild("cylinderRadius") as TextBox;
            cylinderLength = GetTemplateChild("cylinderLength") as TextBox;
            cylinderAxis = GetTemplateChild("cylinderAxis") as ComboBox;
            cylinderEven = GetTemplateChild("cylinderEven") as CheckBox;

            cylinderRadius.InitIntegerField();

            cylinderRadius.SetInteger(radius);
            cylinderLength.SetInteger(length);
            cylinderAxis.SelectedIndex = axis;
            cylinderEven.IsChecked = isEven;

            cylinderRadius.TextChanged += OnRadiusChanged;
            cylinderLength.TextChanged += OnLengthChanged;
            cylinderAxis.SelectionChanged += OnAxisChanged;
            cylinderEven.Checked += OnEvenChanged;
            cylinderEven.Unchecked += OnEvenChanged;
        }

        public int GetRadius()
        {
            return radius;
        }

        public int GetLength()
        {
            return length;
        }

        public int GetAxis()
        {
            return axis;
        }

        public bool IsEven()
        {
            return isEven;
        }

        public void SetValues(int radius, int length, int axis, bool isEven)
        {
            this.radius = radius;
            this.length = length;
            this.axis = axis;
            this.isEven = isEven;
            if(cylinderRadius != null)
            {
                cylinderRadius.SetInteger(radius);
                cylinderLength.SetInteger(length);
                cylinderAxis.SelectedIndex = axis;
                cylinderEven.IsChecked = isEven;
            }
        }

        private void OnRadiusChanged(object sender, TextChangedEventArgs e)
        {
            radius = cylinderRadius.GetInteger(1);
        }

        private void OnLengthChanged(object sender, TextChangedEventArgs e)
        {
            length = cylinderLength.GetInteger(1);
        }

        private void OnAxisChanged(object sender, SelectionChangedEventArgs e)
        {
            axis = cylinderAxis.SelectedIndex;
        }

        private void OnEvenChanged(object sender, RoutedEventArgs e)
        {
            isEven = cylinderEven.IsChecked == true;
        }
    }
}