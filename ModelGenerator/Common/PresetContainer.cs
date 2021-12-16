using Newtonsoft.Json;

namespace ModelGenerator
{
    [JsonObject]
    internal class PresetContainer<T>
    {
        [JsonProperty]
        public string type;
        [JsonProperty]
        public T data;

        public PresetContainer(string type, T data)
        {
            this.type = type;
            this.data = data;
        }
    }

    [JsonObject]
    internal class GeneratorPresetContainer<T> : PresetContainer<T>
    {
        [JsonProperty]
        public string generator;

        public GeneratorPresetContainer(string type, string generator, T data) : base(type, data)
        {
            this.generator = generator;
        }
    }

    internal static class PresetUtility
    {
        public static PresetContainer<T> Get<T>(string type, T data)
        {
            return new PresetContainer<T>(type, data);
        }

        public static GeneratorPresetContainer<T> Get<T>(string type, string generator, T data)
        {
            return new GeneratorPresetContainer<T>(type, generator, data);
        }
    }
}