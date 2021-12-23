using ModelGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Vintagestory.API.MathTools;

namespace VoxelCombinerGenerator
{
    public class VoxelGeneratorSettingsPanel : Control
    {
        public event Action<VoxelGeneratorSettingsPanel> onPanelRemove;

        private ComboBox generatorCombine = null;
        private Vec3iBox generatorOffset = null;
        private ComboBox generatorMaterial = null;
        private Label generatorMaterialLabel = null;
        private Button generatorRemove = null;
        private Expander generatorAnchorExpander = null;

        private Panel generatorAnchor = new DockPanel();

        private VoxelCombineMode combineMode = VoxelCombineMode.DstOverlap;
        private int materialIndex = 0;
        private string generatorTitle = null;
        private Vec3i offset = new Vec3i();
        private CombinerHideControls hiddenControls = CombinerHideControls.None;

        private IObservableReadonlyList<MaterialProperties> materials;

        static VoxelGeneratorSettingsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoxelGeneratorSettingsPanel), new FrameworkPropertyMetadata(typeof(VoxelGeneratorSettingsPanel)));
        }

        public VoxelGeneratorSettingsPanel(IObservableReadonlyList<MaterialProperties> materials)
        {
            this.materials = materials;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            generatorCombine = GetTemplateChild("generatorCombine") as ComboBox;
            generatorOffset = GetTemplateChild("generatorOffset") as Vec3iBox;
            generatorMaterial = GetTemplateChild("generatorMaterial") as ComboBox;
            generatorMaterialLabel = GetTemplateChild("generatorMaterialLabel") as Label;
            generatorRemove = GetTemplateChild("generatorRemove") as Button;
            generatorAnchorExpander = GetTemplateChild("generatorAnchorExpander") as Expander;
            generatorAnchorExpander.Content = generatorAnchor;

            foreach(var field in typeof(VoxelCombineMode).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                generatorCombine.Items.Add(new ComboBoxItem() { Content = attr?.Name ?? field.Name, ToolTip = attr?.Description ?? "" });
            }
            generatorCombine.SelectedIndex = (int)combineMode;
            generatorOffset.SetValue(offset);
            generatorMaterial.ItemsSource = new MaterialNamesCollection(materials);
            generatorMaterial.IsSynchronizedWithCurrentItem = true;
            generatorMaterial.SelectedIndex = materialIndex;
            generatorAnchorExpander.Header = generatorTitle ?? "";

            SetControlsVisibility();

            generatorCombine.SelectionChanged += OnCombineModeChanged;
            generatorMaterial.SelectionChanged += OnMaterialChanged;
            generatorRemove.Click += OnGeneratorRemove;
        }

        public VoxelCombineMode GetCombineMode()
        {
            return combineMode;
        }

        public Vec3i GetOffset()
        {
            return generatorOffset == null ? offset : generatorOffset.GetValue();
        }

        public int GetMaterial()
        {
            return materialIndex;
        }

        public Panel GetGeneratorPanelAnchor()
        {
            return generatorAnchor;
        }

        public void HideControls(CombinerHideControls controls)
        {
            this.hiddenControls = controls;
            if(generatorMaterial != null) SetControlsVisibility();
        }

        public void SetCombineMode(VoxelCombineMode mode)
        {
            this.combineMode = mode;
            if(generatorCombine != null)
            {
                generatorCombine.SelectedIndex = (int)mode;
            }
        }

        public void SetOffset(Vec3i offset)
        {
            if(generatorOffset != null)
            {
                generatorOffset.SetValue(offset);
            }
            else
            {
                this.offset = offset.Clone();
            }
        }

        public void SetMaterial(int materialIndex)
        {
            this.materialIndex = materialIndex;
            if(generatorMaterial != null)
            {
                generatorMaterial.SelectedIndex = materialIndex;
            }
        }

        public void SetGeneratorTitle(string title)
        {
            this.generatorTitle = title;
            if(generatorAnchorExpander != null)
            {
                generatorAnchorExpander.Header = title;
            }
        }

        private void SetControlsVisibility()
        {
            Visibility visibility = (hiddenControls & CombinerHideControls.Material) == 0 ? Visibility.Visible : Visibility.Collapsed;
            generatorMaterial.Visibility = visibility;
            generatorMaterialLabel.Visibility = visibility;
        }

        private void OnCombineModeChanged(object sender, SelectionChangedEventArgs e)
        {
            combineMode = (VoxelCombineMode)generatorCombine.SelectedIndex;
        }

        private void OnMaterialChanged(object sender, SelectionChangedEventArgs e)
        {
            this.materialIndex = generatorMaterial.SelectedIndex;
        }

        private void OnGeneratorRemove(object sender, RoutedEventArgs e)
        {
            onPanelRemove?.Invoke(this);
        }

        private class MaterialNamesCollection : IObservableReadonlyList<string>
        {
            public int Count => materials.Count;

            private IObservableReadonlyList<MaterialProperties> materials;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public MaterialNamesCollection(IObservableReadonlyList<MaterialProperties> materials)
            {
                this.materials = materials;
                materials.CollectionChanged += OnCollectionChanged;
            }

            public string this[int index] => materials[index].name;

            public IEnumerator<string> GetEnumerator()
            {
                foreach(var material in materials)
                {
                    yield return material.name;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach(var material in materials)
                {
                    yield return material.name;
                }
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if(CollectionChanged != null)
                {
                    string[] newItems = null, oldItems = null;
                    if(e.NewItems != null)
                    {
                        newItems = new string[e.NewItems.Count];
                        for(int i = 0; i < newItems.Length; i++)
                        {
                            newItems[i] = ((MaterialProperties)e.NewItems[i]).name;
                        }
                    }
                    if(e.OldItems != null)
                    {
                        oldItems = new string[e.OldItems.Count];
                        for(int i = 0; i < oldItems.Length; i++)
                        {
                            oldItems[i] = ((MaterialProperties)e.OldItems[i]).name;
                        }
                    }
                    NotifyCollectionChangedEventArgs evt = null;
                    switch(e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            evt = new NotifyCollectionChangedEventArgs(e.Action, newItems, e.NewStartingIndex);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            evt = new NotifyCollectionChangedEventArgs(e.Action, oldItems, e.OldStartingIndex);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            evt = new NotifyCollectionChangedEventArgs(e.Action, oldItems, newItems, e.NewStartingIndex);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            evt = new NotifyCollectionChangedEventArgs(e.Action);
                            break;
                        case NotifyCollectionChangedAction.Move:
                            evt = new NotifyCollectionChangedEventArgs(e.Action, oldItems, e.NewStartingIndex, e.OldStartingIndex);
                            break;
                    }
                    CollectionChanged.Invoke(sender, evt);
                }
            }
        }
    }
}