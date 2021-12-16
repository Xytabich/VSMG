using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public partial class MainWindow : Window
    {
        private List<GeneratorInfo> generators = new List<GeneratorInfo>();
        private GeneratorInfo selectedGenerator = null;

        private List<TextureRow> textures = new List<TextureRow>();
        private List<MaterialRow> materials = new List<MaterialRow>();

        private ObservableCollection<string> textureKeysObservable = new ObservableCollection<string>();
        private ObservableCollection<MaterialProperties> materialsObservable;
        private IObservableReadonlyList<MaterialProperties> materialsObservableReadonly;

        public MainWindow()
        {
            InitializeComponent();
            ControlUtils.InitIntegerField(textureWidth);
            ControlUtils.InitIntegerField(textureHeight);

            materialsObservable = new ObservableCollection<MaterialProperties>();
            materialsObservableReadonly = ORLPUtility.Get(materialsObservable);

            AddTextureRow("material", "block/stone/rock/granite1");
            AddMaterialRow(new MaterialProperties() { name = "Material", texture = "material" });

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
                        generators.Add(new GeneratorInfo(attr.name, (IShapeGenerator)Activator.CreateInstance(type, true)));
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
                row.panel.Dispose();
                materialList.Children.Remove(row.root);
            }
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

        private class GeneratorInfo
        {
            public string name;
            public IShapeGenerator instance;

            public GeneratorInfo(string name, IShapeGenerator instance)
            {
                this.name = name;
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
    }
}