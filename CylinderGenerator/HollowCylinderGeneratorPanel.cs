using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace CylinderGenerator
{
    public class HollowCylinderGeneratorPanel : CylinderGeneratorPanel
    {
        private TextBox cylinderInnerRadius;

        private int innerRadius = 1;

        static HollowCylinderGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HollowCylinderGeneratorPanel), new FrameworkPropertyMetadata(typeof(HollowCylinderGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            cylinderInnerRadius = GetTemplateChild("cylinderInnerRadius") as TextBox;

            cylinderInnerRadius.InitIntegerField();
            cylinderInnerRadius.SetInteger(innerRadius);

            cylinderInnerRadius.TextChanged += OnRadiusChanged;
        }

        public int GetInnerRadius()
        {
            return innerRadius;
        }

        public void SetValues(Vec3d offset, int outerRadius, int innerRadius, int length, int axis, bool isEven)
        {
            base.SetValues(offset, outerRadius, length, axis, isEven);

            this.innerRadius = innerRadius;
            if(cylinderInnerRadius != null)
            {
                cylinderInnerRadius.SetInteger(innerRadius);
            }
        }

        private void OnRadiusChanged(object sender, TextChangedEventArgs e)
        {
            innerRadius = cylinderInnerRadius.GetInteger(1);
        }
    }
}