using ModelGenerator;
using System.Collections.Generic;

namespace VoxelCombinerGenerator
{
    public struct VoxelGeneratorContext
    {
        public object generatorData;
        public int materialIndex;
        public IReadOnlyList<MaterialProperties> materials;
    }
}