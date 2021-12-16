namespace ModelGenerator
{
    public interface IShapeGenerator
    {
        void ShowPanel(EditorContext context);

        void OnHide();

        void Generate(GeneratorContext context);
    }

    public interface IPresetShapeGenerator : IShapeGenerator
    {
        void ApplyPreset(object preset);

        object CreatePreset();
    }
}