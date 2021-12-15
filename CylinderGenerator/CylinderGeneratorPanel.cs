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

            ControlUtils.InitIntegerField(cylinderRadius);
        }

        public Vec3d GetOffset()
        {
            return cylinderOffset.GetValue();
        }

        public int GetRadius()
        {
            return cylinderRadius.GetInteger(1);
        }

        public int GetLength()
        {
            return cylinderLength.GetInteger(1);
        }

        public int GetAxis()
        {
            return cylinderAxis.SelectedIndex;
        }

        public bool IsEven()
        {
            return cylinderEven.IsChecked == true;
        }
    }
}