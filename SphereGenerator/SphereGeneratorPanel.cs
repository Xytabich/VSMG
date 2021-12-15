using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace SphereGenerator
{
    public class SphereGeneratorPanel : Control
    {
        private TextBox sphereRadius;
        private CheckBox sphereEven;
        private Vec3dBox sphereOffset;

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

            ControlUtils.InitIntegerField(sphereRadius);
        }

        public Vec3d GetOffset()
        {
            return sphereOffset.GetValue();
        }

        public int GetRadius()
        {
            return sphereRadius.GetInteger(1);
        }

        public bool IsEven()
        {
            return sphereEven.IsChecked == true;
        }
    }
}