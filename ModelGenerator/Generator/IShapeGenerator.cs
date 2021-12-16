namespace ModelGenerator
{
    public interface IShapeGenerator
    {
        void ShowPanel(EditorContext context);

        void OnHide();

        void Generate(GeneratorContext context);
    }
}