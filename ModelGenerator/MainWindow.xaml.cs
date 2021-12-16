using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public partial class MainWindow : Window
    {
        private const string SHAPE_PRESET_TYPE = "shape";
        private const string GENERATOR_PRESET_TYPE = "generator";

        public ICommand saveCommand { get; }
        public ICommand loadCommand { get; }

        private Dictionary<string, GeneratorInfo> key2Generator = new Dictionary<string, GeneratorInfo>();
        private List<GeneratorInfo> generators = new List<GeneratorInfo>();
        private GeneratorInfo selectedGenerator = null;

        private List<TextureRow> textures = new List<TextureRow>();
        private List<MaterialRow> materials = new List<MaterialRow>();

        private ObservableCollection<string> textureKeysObservable = new ObservableCollection<string>();
        private ObservableCollection<MaterialProperties> materialsObservable;
        private IObservableReadonlyList<MaterialProperties> materialsObservableReadonly;

        private List<string> shapePresetNames = new List<string>();
        private List<ShapePresetData> shapePresets = new List<ShapePresetData>();
        private Dictionary<string, int> path2ShapePresetIndex = new Dictionary<string, int>();

        private List<string> generatorPresetNames = new List<string>();
        private Dictionary<string, object> generatorPresets = new Dictionary<string, object>();

        public MainWindow()
        {
            saveCommand = new SaveCommand(this);
            loadCommand = new LoadCommand(this);

            InitializeComponent();
            textureWidth.InitIntegerField();
            textureHeight.InitIntegerField();

            materialsObservable = new ObservableCollection<MaterialProperties>();
            materialsObservableReadonly = ORLPUtility.Get(materialsObservable);

            AddTextureRow("material", "block/stone/rock/granite1");
            AddMaterialRow(new MaterialProperties() { name = "Material", texture = "material" });

            generatorPresetNames.Add("Copy from preset...");
            generatorPreset.ItemsSource = generatorPresetNames;
            generatorPreset.SelectedIndex = 0;
            generatorPreset.IsEnabled = false;

            shapePresetNames.Add("Copy from preset...");
            shapePreset.ItemsSource = shapePresetNames;
            shapePreset.SelectedIndex = 0;
            shapePreset.IsEnabled = false;

            generatorPreset.SelectionChanged += OnGeneratorPresetSelected;
            shapePreset.SelectionChanged += OnShapePresetSelected;
            textureListAdd.Click += AddEmptyTexture;
            materialListAdd.Click += AddMaterial;
            generate.Click += OnGenerate;

            var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generators"));
            if(!dir.Exists) dir.Create();
            foreach(var module in dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                var asm = Assembly.LoadFile(module.FullName);
                foreach(var type in asm.GetTypes())
                {
                    var attr = type.GetCustomAttribute<ShapeGeneratorAttribute>();
                    if(attr != null && typeof(IShapeGenerator).IsAssignableFrom(type))
                    {
                        Type presetType = attr.presetType;
                        if(presetType != null && !typeof(IPresetShapeGenerator).IsAssignableFrom(type))
                        {
                            presetType = null;
                        }
                        string key = type.ToString();
                        var info = new GeneratorInfo(key, attr.name, presetType, (IShapeGenerator)Activator.CreateInstance(type, true));
                        generators.Add(info);
                        key2Generator[key] = info;
                    }
                }
            }
            for(int i = 0; i < generators.Count; i++)
            {
                generatorSelect.Items.Add(new ComboBoxItem() { Content = generators[i].name });
            }
            if(generators.Count > 0)
            {
                generatorSelect.SelectedIndex = 0;
                ShowGeneratorPanel(0);
                generatorSelect.SelectionChanged += OnGeneratorSelected;
            }
            else
            {
                generatorSelect.SelectedIndex = -1;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Properties.Settings.Default.Save();
        }

        private void SaveShapePreset()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "JSON|*.json";
            if(dialog.ShowDialog(GetWindow(this)) == true)
            {
                var preset = new ShapePresetData();
                preset.textureWidth = textureWidth.GetInteger();
                preset.textureHeight = textureHeight.GetInteger();

                preset.textures = new Dictionary<string, string>();
                foreach(var texture in textures)
                {
                    preset.textures[texture.keyText.Text.Trim()] = texture.pathText.Text.Trim();
                }

                preset.applyRoot = applyRoot.IsChecked == true;
                preset.offsetChildren = offsetChildren.IsChecked == true;
                preset.rootOffset = rootOffset.GetValue().Clone();

                preset.materials = new List<MaterialProperties>(materialsObservable.Count);
                foreach(var material in materialsObservable)
                {
                    preset.materials.Add(material.Clone());
                }

                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(PresetUtility.Get(SHAPE_PRESET_TYPE, preset), new JsonSerializerSettings {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                }));

                AddShapePreset(dialog.FileName, preset);
            }
        }

        private void SaveGeneratorPreset()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "JSON|*.json";
            if(dialog.ShowDialog(GetWindow(this)) == true)
            {
                var preset = (selectedGenerator.instance as IPresetShapeGenerator).CreatePreset();

                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(PresetUtility.Get(GENERATOR_PRESET_TYPE, selectedGenerator.key, preset), new JsonSerializerSettings {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                }));

                AddGeneratorPreset(selectedGenerator, dialog.FileName, preset);
            }
        }

        private void ShowLoadPresetsDialog()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "JSON|*.json";
            if(dialog.ShowDialog(GetWindow(this)) == true)
            {
                foreach(var path in dialog.FileNames)
                {
                    try
                    {
                        JObject json = JObject.Parse(File.ReadAllText(path));
                        if(json.TryGetValue("type", StringComparison.InvariantCultureIgnoreCase, out var type) && type.Type == JTokenType.String)
                        {
                            switch((string)type)
                            {
                                case SHAPE_PRESET_TYPE:
                                    AddShapePreset(path, json["data"].ToObject<ShapePresetData>());
                                    break;
                                case GENERATOR_PRESET_TYPE:
                                    if(key2Generator.TryGetValue((string)json["generator"], out var generator) && generator.presetType != null)
                                    {
                                        AddGeneratorPreset(generator, path, json["data"].ToObject(generator.presetType));
                                    }
                                    break;
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void AddGeneratorPreset(GeneratorInfo generator, string path, object preset)
        {
            path = Path.GetFullPath(path);
            if(!generatorPresets.ContainsKey(path))
            {
                generator.presets.Add(path);
                if(generator == selectedGenerator)
                {
                    generatorPresetNames.Add(Path.GetFileNameWithoutExtension(path));
                    generatorPreset.IsEnabled = true;
                }
            }
            generatorPresets[path] = preset;
        }

        private void OnGeneratorPresetSelected(object sender, SelectionChangedEventArgs e)
        {
            int index = generatorPreset.SelectedIndex - 1;
            if(index >= 0)
            {
                generatorPreset.SelectedIndex = 0;
                (selectedGenerator.instance as IPresetShapeGenerator).ApplyPreset(generatorPresets[selectedGenerator.presets[index]]);
            }
        }

        private void AddShapePreset(string path, ShapePresetData preset)
        {
            path = Path.GetFullPath(path);
            if(path2ShapePresetIndex.TryGetValue(path, out int index))
            {
                shapePresets[index] = preset;
            }
            else
            {
                path2ShapePresetIndex.Add(path, shapePresets.Count);
                shapePresets.Add(preset);
                shapePresetNames.Add(Path.GetFileNameWithoutExtension(path));
                shapePreset.IsEnabled = true;
            }
        }

        private void OnShapePresetSelected(object sender, SelectionChangedEventArgs e)
        {
            int index = shapePreset.SelectedIndex - 1;
            if(index >= 0)
            {
                shapePreset.SelectedIndex = 0;

                var preset = shapePresets[index];
                textureWidth.SetInteger(preset.textureWidth);
                textureHeight.SetInteger(preset.textureHeight);

                ClearMaterials();
                ClearTextures();

                foreach(var texture in preset.textures)
                {
                    AddTextureRow(texture.Key, texture.Value);
                }

                applyRoot.IsChecked = preset.applyRoot;
                offsetChildren.IsChecked = preset.offsetChildren;
                rootOffset.SetValue(preset.rootOffset);

                foreach(var material in preset.materials)
                {
                    AddMaterialRow(material.Clone());
                }
            }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            if(selectedGenerator == null) return;
            var dialog = new SaveFileDialog();
            dialog.Filter = "JSON|*.json";
            if(dialog.ShowDialog(GetWindow(this)) == true)
            {
                var shape = new Shape() { TextureWidth = textureWidth.GetInteger(16), TextureHeight = textureHeight.GetInteger(16) };
                shape.Textures = new Dictionary<string, AssetLocation>();
                foreach(var texture in textures)
                {
                    shape.Textures[texture.keyText.Text.Trim()] = new AssetLocation(texture.pathText.Text.Trim().Replace('\\', '/'));
                }
                selectedGenerator.instance.Generate(new GeneratorContext() { shape = shape, materials = materialsObservable });
                if(applyRoot.IsChecked == true)
                {
                    var offset = rootOffset.GetValue();
                    if(shape.Elements != null && offsetChildren.IsChecked != true)
                    {
                        foreach(var element in shape.Elements)
                        {
                            element.From[0] -= offset.X;
                            element.From[1] -= offset.Y;
                            element.From[2] -= offset.Z;
                            element.To[0] -= offset.X;
                            element.To[1] -= offset.Y;
                            element.To[2] -= offset.Z;
                        }
                    }
                    var root = new ShapeElement();
                    root.Name = "root";
                    root.From = new double[] { offset.X, offset.Y, offset.Z };
                    root.To = new double[] { offset.X, offset.Y, offset.Z };
                    root.Faces = new Dictionary<string, ShapeElementFace>() {
                        { BlockFacing.EAST.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } },
                        { BlockFacing.WEST.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } },
                        { BlockFacing.UP.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } },
                        { BlockFacing.DOWN.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } },
                        { BlockFacing.SOUTH.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } },
                        { BlockFacing.NORTH.Code, new ShapeElementFace() { Texture = "#null", Uv = new float[4], Enabled = false } }
                    };
                    root.Children = shape.Elements;
                    shape.Elements = new ShapeElement[] { root };
                }
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(shape, new JsonSerializerSettings {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
        }

        private void OnGeneratorSelected(object sender, SelectionChangedEventArgs e)
        {
            ShowGeneratorPanel(generatorSelect.SelectedIndex);
        }

        private void ShowGeneratorPanel(int index)
        {
            if(selectedGenerator != null)
            {
                selectedGenerator.instance.OnHide();
                generatorPanel.Children.Clear();
            }
            selectedGenerator = generators[index];
            selectedGenerator.instance.ShowPanel(new EditorContext() { parent = generatorPanel, materials = materialsObservableReadonly });
            for(int i = generatorPresetNames.Count - 1; i > 0; i--)
            {
                generatorPresetNames.RemoveAt(i);
            }
            foreach(var path in selectedGenerator.presets)
            {
                generatorPresetNames.Add(Path.GetFileNameWithoutExtension(path));
            }
            generatorPreset.IsEnabled = selectedGenerator.presets.Count > 0;
        }

        private void AddMaterial(object sender, RoutedEventArgs e)
        {
            AddMaterialRow(new MaterialProperties() { name = "Material", texture = textures.Count > 0 ? textures[0].keyText.Text.Trim() : "" });
        }

        private void AddMaterialRow(MaterialProperties material)
        {
            Button removeBtn = new Button() { Content = "X", Width = 26, Height = 22, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
            DockPanel.SetDock(removeBtn, Dock.Right);
            MaterialPanel panel = new MaterialPanel(material, textureKeysObservable);

            var root = new DockPanel();
            root.Children.Add(removeBtn);
            root.Children.Add(panel);
            materialList.Children.Add(root);
            materials.Add(new MaterialRow(panel, removeBtn, root, RemoveMaterial, OnMaterialNameChanged));
            materialsObservable.Add(material);
        }

        private void OnMaterialNameChanged(MaterialRow row)
        {
            int index = materials.IndexOf(row);
            if(index >= 0)
            {
                materialsObservable[index] = materialsObservable[index];
            }
        }

        private void RemoveMaterial(MaterialRow row)
        {
            int index = materials.IndexOf(row);
            if(index >= 0)
            {
                materials.RemoveAt(index);
                materialsObservable.RemoveAt(index);
                materialList.Children.Remove(row.root);
            }
        }

        private void ClearMaterials()
        {
            materials.Clear();
            materialsObservable.Clear();
            materialList.Children.Clear();
        }

        private void AddEmptyTexture(object sender, RoutedEventArgs e)
        {
            AddTextureRow("", "");
        }

        private void AddTextureRow(string key, string path)
        {
            Button removeBtn;
            TextBox keyText, pathText;
            textureListKeys.Children.Add(keyText = new TextBox());
            textureListPaths.Children.Add(pathText = new TextBox());
            textureListButtons.Children.Add(removeBtn = new Button());
            keyText.Text = key;
            pathText.Text = path;
            removeBtn.Content = "X";
            keyText.Height = pathText.Height = removeBtn.Height = 20;
            textures.Add(new TextureRow(keyText, pathText, removeBtn, RemoveTexture, OnTextureKeyChanged));
            textureKeysObservable.Add(key.Trim());
        }

        private void OnTextureKeyChanged(TextureRow row)
        {
            textureKeysObservable[textures.IndexOf(row)] = row.keyText.Text.Trim();
        }

        private void RemoveTexture(TextureRow row)
        {
            int index = textures.IndexOf(row);
            if(index >= 0)
            {
                textures.RemoveAt(index);
                textureListKeys.Children.Remove(row.keyText);
                textureListPaths.Children.Remove(row.pathText);
                textureListButtons.Children.Remove(row.removeBtn);
                textureKeysObservable.RemoveAt(index);
            }
        }

        private void ClearTextures()
        {
            textures.Clear();
            textureListKeys.Children.Clear();
            textureListPaths.Children.Clear();
            textureListButtons.Children.Clear();
            textureKeysObservable.Clear();
        }

        private class GeneratorInfo
        {
            public string key;
            public string name;
            public Type presetType;
            public IShapeGenerator instance;
            public List<string> presets = new List<string>();

            public GeneratorInfo(string key, string name, Type presetType, IShapeGenerator instance)
            {
                this.key = key;
                this.name = name;
                this.presetType = presetType;
                this.instance = instance;
            }
        }

        private class TextureRow
        {
            public readonly TextBox keyText;
            public readonly TextBox pathText;
            public readonly Button removeBtn;

            private Action<TextureRow> onRemove;
            private Action<TextureRow> onChanged;

            public TextureRow(TextBox keyText, TextBox pathText, Button removeBtn, Action<TextureRow> onRemove, Action<TextureRow> onChanged)
            {
                this.keyText = keyText;
                this.pathText = pathText;
                this.removeBtn = removeBtn;
                this.onRemove = onRemove;
                this.onChanged = onChanged;
                removeBtn.Click += OnRemove;
                keyText.TextChanged += OnChanged;
            }

            private void OnChanged(object sender, TextChangedEventArgs e)
            {
                onChanged.Invoke(this);
            }

            private void OnRemove(object sender, RoutedEventArgs e)
            {
                onRemove.Invoke(this);
            }
        }

        private class MaterialRow
        {
            public MaterialPanel panel;
            public Button removeBtn;
            public Panel root;

            private Action<MaterialRow> onRemove;
            private Action<MaterialRow> onChanged;

            public MaterialRow(MaterialPanel panel, Button removeBtn, Panel root, Action<MaterialRow> onRemove, Action<MaterialRow> onChanged)
            {
                this.panel = panel;
                this.removeBtn = removeBtn;
                this.root = root;
                this.onRemove = onRemove;
                this.onChanged = onChanged;
                panel.onNameChanged = OnChanged;
                removeBtn.Click += OnRemove;
            }

            private void OnChanged()
            {
                onChanged.Invoke(this);
            }

            private void OnRemove(object sender, RoutedEventArgs e)
            {
                onRemove.Invoke(this);
            }
        }

        private class SaveCommand : ICommand
        {
            private MainWindow window;

            public event EventHandler CanExecuteChanged;

            public SaveCommand(MainWindow window)
            {
                this.window = window;
            }

            public bool CanExecute(object parameter)
            {
                if(parameter is string str)
                {
                    if(str == "generator")
                    {
                        return window.selectedGenerator != null && window.selectedGenerator.presetType != null;
                    }
                    return true;
                }
                return false;
            }

            public void Execute(object parameter)
            {
                switch((string)parameter)
                {
                    case "generator":
                        window.SaveGeneratorPreset();
                        break;
                    case "shape":
                        window.SaveShapePreset();
                        break;
                }
            }
        }

        private class LoadCommand : ICommand
        {
            private MainWindow window;

            public event EventHandler CanExecuteChanged;

            public LoadCommand(MainWindow window)
            {
                this.window = window;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                window.ShowLoadPresetsDialog();
            }
        }
    }
}