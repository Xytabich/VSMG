using System.Collections.Generic;
using Vintagestory.API.Common;

namespace ModelGenerator
{
    public struct GeneratorContext
    {
        public Shape shape;
        public IReadOnlyList<MaterialProperties> materials;
    }
}