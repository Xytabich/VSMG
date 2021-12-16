using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    [JsonObject]
    internal class ShapePresetData
    {
        [JsonProperty]
        public int textureWidth;

        [JsonProperty]
        public int textureHeight;

        [JsonProperty]
        public Dictionary<string, string> textures;

        [JsonProperty]
        public bool applyRoot;

        [JsonProperty]
        public bool offsetChildren;

        [JsonProperty]
        public Vec3d rootOffset;

        [JsonProperty]
        public List<MaterialProperties> materials;
    }
}