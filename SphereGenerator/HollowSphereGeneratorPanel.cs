using ModelGenerator;
using System.Windows;
using System.Windows.Controls;

namespace SphereGenerator
{
    public class HollowSphereGeneratorPanel : SphereGeneratorPanel
    {
        private TextBox innerRadius;

        static HollowSphereGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HollowSphereGeneratorPanel), new FrameworkPropertyMetadata(typeof(HollowSphereGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            innerRadius = GetTemplateChild("sphereInnerRadius") as TextBox;
            ControlUtils.InitIntegerField(innerRadius);
        }

        public int GetInnerRadius()
        {
            return innerRadius.GetInteger(1);
        }
    }
}