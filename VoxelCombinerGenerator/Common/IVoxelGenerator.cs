using ModelGenerator;

namespace VoxelCombinerGenerator
{
    public interface IVoxelGenerator
    {
        void CreatePanel(EditorContext context, out object generatorData);

        void OnPanelDestroyed(object generatorData);

        object CreateSaveData(object generatorData);

        void ApplySaveData(object generatorData, object saveData);

        int[,,] Generate(VoxelGeneratorContext context);
    }
}