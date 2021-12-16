using System.Windows.Controls;

namespace ModelGenerator
{
    public struct EditorContext
    {
        public Panel parent;
        public IObservableReadonlyList<MaterialProperties> materials;
    }
}