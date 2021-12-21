using ModelGenerator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Vintagestory.API.MathTools;

namespace VoxelCombinerGenerator
{
    [VoxelGenerator("Voxel Combiner", typeof(GeneratorSave), CombinerHideControls.Material)]
    public class VoxelCombiner : IVoxelGenerator
    {
        private static Dictionary<string, int> key2Generator = null;
        private static SourceGeneratorInfo[] generators = null;
        private static string[] generatorNames = null;

        void IVoxelGenerator.CreatePanel(EditorContext context, out object generatorData)
        {
            CreatePanel(context, out generatorData);
        }

        void IVoxelGenerator.OnPanelDestroyed(object generatorData)
        {
            OnPanelDestroyed(generatorData);
        }

        void IVoxelGenerator.ApplySaveData(object generatorData, object saveData)
        {
            ApplySaveData(generatorData, saveData);
        }

        object IVoxelGenerator.CreateSaveData(object generatorData)
        {
            return CreateSaveData(generatorData);
        }

        int[,,] IVoxelGenerator.Generate(VoxelGeneratorContext context)
        {
            return Generate(context);
        }

        public static void CreatePanel(EditorContext context, out object generatorData)
        {
            var panel = new VoxelCombinerPanel();
            context.parent.Children.Add(panel);
            generatorData = new GeneratorInstance(panel, context);
        }

        public static void OnPanelDestroyed(object generatorData)
        {
            ((GeneratorInstance)generatorData).OnHide();
        }

        public static void ApplySaveData(object generatorData, object saveData)
        {
            ((GeneratorInstance)generatorData).ApplySave((GeneratorSave)saveData);
        }

        public static object CreateSaveData(object generatorData)
        {
            return ((GeneratorInstance)generatorData).CreateSave();
        }

        public static int[,,] Generate(VoxelGeneratorContext context)
        {
            throw new NotImplementedException();
        }

        private static void InitGenerators()
        {
            if(generators != null) return;

            var asm = typeof(VoxelGeneratorAttribute).Assembly;
            var name = asm.GetName();
            var list = new List<SourceGeneratorInfo>();
            key2Generator = new Dictionary<string, int>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly == asm || Array.Exists(assembly.GetReferencedAssemblies(), a => AssemblyName.ReferenceMatchesDefinition(a, name)))
                {
                    foreach(var type in assembly.GetTypes())
                    {
                        var attr = type.GetCustomAttribute<VoxelGeneratorAttribute>();
                        if(attr != null && typeof(IVoxelGenerator).IsAssignableFrom(type))
                        {
                            string key = type.ToString();
                            key2Generator.Add(key, list.Count);
                            list.Add(new SourceGeneratorInfo() {
                                key = key,
                                name = attr.name,
                                hideControls = attr.hideControls,
                                saveDataType = attr.saveDataType,
                                instance = (IVoxelGenerator)Activator.CreateInstance(type)
                            });
                        }
                    }
                }
            }
            generators = list.ToArray();
            generatorNames = new string[list.Count];
            for(int i = 0; i < generatorNames.Length; i++)
            {
                generatorNames[i] = list[i].name;
            }
        }

        private class GeneratorInstance
        {
            public VoxelCombinerPanel panel;

            private EditorContext context;
            private List<SourceGeneratorInstance> sources = new List<SourceGeneratorInstance>();

            public GeneratorInstance(VoxelCombinerPanel panel, EditorContext context)
            {
                this.panel = panel;
                this.context = context;
                panel.onAddClick += SelectGenerator;
            }

            public void OnHide()
            {
                foreach(var source in sources)
                {
                    source.generator.OnPanelDestroyed(source.generatorData);
                }
            }

            public void ApplySave(GeneratorSave saveData)
            {
                foreach(var source in sources)
                {
                    panel.RemoveGenerator(source.panel);
                    source.generator.OnPanelDestroyed(source.generatorData);
                }
                sources.Clear();
                InitGenerators();
                foreach(var source in saveData.sources)
                {
                    if(key2Generator.TryGetValue(source.generator, out var index))
                    {
                        var generator = generators[index];
                        var instance = AddGeneratorInstance(generator);
                        instance.panel.SetCombineMode(source.combineMode);
                        instance.panel.SetOffset(source.offset);
                        int materialIndex = 0;
                        if(!string.IsNullOrEmpty(source.material))
                        {
                            for(int i = 0; i < context.materials.Count; i++)
                            {
                                if(context.materials[i].name == source.material)
                                {
                                    materialIndex = i;
                                    break;
                                }
                            }
                        }
                        instance.panel.SetMaterial(materialIndex);
                        if(source.data != null && generator.saveDataType.IsAssignableFrom(source.data.GetType()))
                        {
                            instance.generator.ApplySaveData(instance.generatorData, source.data);
                        }
                    }
                }
            }

            public object CreateSave()
            {
                var sources = new GeneratorSave.SourceGenerator[this.sources.Count];
                for(int i = this.sources.Count - 1; i >= 0; i--)
                {
                    var source = this.sources[i];
                    var panel = source.panel;
                    sources[i] = new GeneratorSave.SourceGenerator() {
                        generator = source.generatorInfo.key,
                        combineMode = panel.GetCombineMode(),
                        offset = panel.GetOffset().Clone(),
                        material = context.materials[panel.GetMaterial()].name,
                        data = source.generator.CreateSaveData(source.generatorData)
                    };
                }
                return new GeneratorSave() { sources = sources };
            }

            private void SelectGenerator()
            {
                InitGenerators();
                panel.ShowAddMenu(generatorNames, AddGenerator);
            }

            private void AddGenerator(int index)
            {
                AddGeneratorInstance(generators[index]);
            }

            private SourceGeneratorInstance AddGeneratorInstance(SourceGeneratorInfo generator)
            {
                var panel = new VoxelGeneratorSettingsPanel(context.materials);
                panel.HideControls(generator.hideControls);
                panel.SetGeneratorTitle(generator.name);
                this.panel.AddGenerator(panel);
                panel.ApplyTemplate();//TODO: maybe not work if expander is closed

                context.parent = panel.GetGeneratorPanelAnchor();
                generator.instance.CreatePanel(context, out var data);
                panel.onPanelRemove += RemovePanel;

                var instance = new SourceGeneratorInstance() {
                    panel = panel,
                    generatorInfo = generator,
                    generatorData = data
                };
                sources.Add(instance);

                return instance;
            }

            private void RemovePanel(VoxelGeneratorSettingsPanel panel)
            {
                int index = sources.FindIndex(s => s.panel == panel);
                if(index >= 0)
                {
                    sources[index].OnRemoved();
                    this.panel.RemoveGenerator(panel);
                    sources.RemoveAt(index);
                }
            }

            private class SourceGeneratorInstance
            {
                public SourceGeneratorInfo generatorInfo;
                public IVoxelGenerator generator => generatorInfo.instance;
                public VoxelGeneratorSettingsPanel panel;
                public object generatorData;

                public void OnRemoved()
                {
                    generator.OnPanelDestroyed(generatorData);
                }
            }
        }

        [JsonObject]
        internal class GeneratorSave
        {
            [JsonProperty]
            public SourceGenerator[] sources;

            [JsonObject, JsonConverter(typeof(SourceGeneratorSaveConverter))]
            public class SourceGenerator
            {
                [JsonProperty]
                public string generator;
                [JsonProperty]
                public Vec3i offset = new Vec3i();
                [JsonProperty]
                public VoxelCombineMode combineMode = VoxelCombineMode.DstOverlap;
                [JsonProperty]
                public string material;
                [JsonProperty]
                public object data;
            }
        }

        private class SourceGeneratorInfo
        {
            public string key;
            public string name;
            public Type saveDataType;
            public CombinerHideControls hideControls;
            public IVoxelGenerator instance;
        }

        private class SourceGeneratorSaveConverter : JsonConverter
        {
            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(GeneratorSave.SourceGenerator);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var json = JObject.Load(reader);
                if(json.TryGetValue(nameof(GeneratorSave.SourceGenerator.generator), StringComparison.InvariantCultureIgnoreCase, out var generator))
                {
                    InitGenerators();
                    var key = (string)generator;
                    if(key2Generator.TryGetValue(key, out var index))
                    {
                        var source = new GeneratorSave.SourceGenerator();
                        source.generator = key;
                        if(json.TryGetValue(nameof(GeneratorSave.SourceGenerator.offset), StringComparison.InvariantCultureIgnoreCase, out var offset))
                        {
                            source.offset = offset.ToObject<Vec3i>();
                        }
                        if(json.TryGetValue(nameof(GeneratorSave.SourceGenerator.combineMode), StringComparison.InvariantCultureIgnoreCase, out var combineMode))
                        {
                            source.combineMode = combineMode.ToObject<VoxelCombineMode>();
                        }
                        if(json.TryGetValue(nameof(GeneratorSave.SourceGenerator.material), StringComparison.InvariantCultureIgnoreCase, out var material))
                        {
                            source.material = (string)material;
                        }
                        if(json.TryGetValue(nameof(GeneratorSave.SourceGenerator.data), StringComparison.InvariantCultureIgnoreCase, out var data))
                        {
                            var dataType = generators[index].saveDataType;
                            if(dataType != null)
                            {
                                source.data = data.ToObject(dataType, serializer);
                            }
                        }
                        return source;
                    }
                }
                reader.Skip();
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}