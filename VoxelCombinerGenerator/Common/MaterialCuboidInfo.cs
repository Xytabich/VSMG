using ModelGenerator;

namespace VoxelCombinerGenerator
{
    internal class MaterialCuboidInfo : CuboidInfo
    {
        public int material;

        public MaterialCuboidInfo(int x, int y, int z, int sizeX, int sizeY, int sizeZ) : base(x, y, z, sizeX, sizeY, sizeZ) { }
    }
}
