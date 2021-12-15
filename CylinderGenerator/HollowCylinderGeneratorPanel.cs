using ModelGenerator;
using System.Windows;
using System.Windows.Controls;

namespace CylinderGenerator
{
    public class HollowCylinderGeneratorPanel : CylinderGeneratorPanel
    {
        private TextBox innerRadius;

        static HollowCylinderGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HollowCylinderGeneratorPanel), new FrameworkPropertyMetadata(typeof(HollowCylinderGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            innerRadius = GetTemplateChild("cylinderInnerRadius") as TextBox;
            ControlUtils.InitIntegerField(innerRadius);
        }

        public int GetInnerRadius()
        {
            return innerRadius.GetInteger(1);
        }
    }
}