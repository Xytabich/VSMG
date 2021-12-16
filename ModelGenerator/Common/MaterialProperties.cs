using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace ModelGenerator
{
    [JsonObject]
    public class MaterialProperties
    {
        [JsonProperty]
        public string name = null;

        [JsonProperty]
        public string texture = null;

        [JsonProperty]
        public bool shade = true;

        [JsonProperty]
        public int glow = 0;

        [JsonProperty]
        public string climateColorMap = null;

        [JsonProperty]
        public string seasonColorMap = null;

        [JsonProperty]
        public short renderPass = 0;//EnumChunkRenderPass

        [JsonProperty]
        public short zOffset = 0;

        [JsonProperty]
        public bool waterWave = false;

        [JsonProperty]
        public bool reflective = false;

        public void ApplyFaceProperties(ShapeElementFace face)
        {
            face.Glow = glow;
            face.Texture = string.IsNullOrEmpty(texture) ? "#null" : ("#" + texture);
        }

        public void ApplyCuboidProperties(ShapeElement cuboid)
        {
            cuboid.Shade = shade;
            cuboid.ClimateColorMap = climateColorMap;
            cuboid.SeasonColorMap = seasonColorMap;
            cuboid.RenderPass = renderPass;
            cuboid.ZOffset = zOffset;
            cuboid.WaterWave = waterWave;
            cuboid.Reflective = reflective;
        }

        public void ApplyAllProperties(ShapeElement element)
        {
            ApplyCuboidProperties(element);
            foreach(var pair in element.Faces)
            {
                ApplyFaceProperties(pair.Value);
            }
        }
    }
}