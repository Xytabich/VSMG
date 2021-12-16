using ModelGenerator;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace SphereGenerator
{
    public class HollowSphereGeneratorPanel : SphereGeneratorPanel
    {
        private TextBox sphereInnerRadius;

        private int innerRadius = 1;

        static HollowSphereGeneratorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HollowSphereGeneratorPanel), new FrameworkPropertyMetadata(typeof(HollowSphereGeneratorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            sphereInnerRadius = GetTemplateChild("sphereInnerRadius") as TextBox;

            sphereInnerRadius.InitIntegerField();
            sphereInnerRadius.SetInteger(innerRadius);

            sphereInnerRadius.TextChanged += OnRadiusChanged;
        }

        public int GetInnerRadius()
        {
            return innerRadius;
        }

        public void SetValues(Vec3d offset, int outerRadius, int innerRadius, bool isEven)
        {
            base.SetValues(offset, outerRadius, isEven);

            this.innerRadius = innerRadius;
            if(sphereInnerRadius != null)
            {
                sphereInnerRadius.SetInteger(innerRadius);
            }
        }

        private void OnRadiusChanged(object sender, TextChangedEventArgs e)
        {
            innerRadius = sphereInnerRadius.GetInteger(1);
        }
    }
}