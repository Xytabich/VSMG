using System.Windows.Controls;
using Vintagestory.API.Common;

namespace ModelGenerator
{
    public interface IShapeGenerator
    {
        void ShowPanel(Panel parent);

        void OnHide();

        void Generate(Shape shape);
    }
}