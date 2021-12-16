using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace CylinderGenerator
{
    public class CylinderGeneratorPanel : Control
    {
        private TextBox cylinderRadius;
        private TextBox cylinderLength;
        private ComboBox cylinderAxis;
        private CheckBox cylinderEven;
        private Vec3dBox cylinderOffset;

        private int radius = 1;
        private int length = 1;
        private int axis = 0;
        private bool isEven = true;
        private Vec3d offset = Vec3d.Zero;

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
            cylinderOffset = GetTemplateChild("cylinderOffset") as Vec3dBox;

            cylinderRadius.InitIntegerField();

            cylinderRadius.SetInteger(radius);
            cylinderLength.SetInteger(length);
            cylinderAxis.SelectedIndex = axis;
            cylinderEven.IsChecked = isEven;
            cylinderOffset.SetValue(offset);

            cylinderRadius.TextChanged += OnRadiusChanged;
            cylinderLength.TextChanged += OnLengthChanged;
            cylinderAxis.SelectionChanged += OnAxisChanged;
            cylinderEven.Checked += OnEvenChanged;
            cylinderEven.Unchecked += OnEvenChanged;
        }

        public Vec3d GetOffset()
        {
            return cylinderOffset == null ? offset : cylinderOffset.GetValue();
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

        public void SetValues(Vec3d offset, int radius, int length, int axis, bool isEven)
        {
            this.radius = radius;
            this.length = length;
            this.axis = axis;
            this.isEven = isEven;
            this.offset = offset.Clone();
            if(cylinderRadius != null)
            {
                cylinderRadius.SetInteger(radius);
                cylinderLength.SetInteger(length);
                cylinderAxis.SelectedIndex = axis;
                cylinderEven.IsChecked = isEven;
                cylinderOffset.SetValue(offset);
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