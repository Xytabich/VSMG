using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ModelGenerator
{
    public class MaterialPanel : Control, IDisposable
    {
        public Action onNameChanged = null;

        private MaterialProperties material;
        private ObservableCollection<string> textureKeys;

        private TextBox materialName;
        private ComboBox materialTexture;
        private CheckBox materialShade;
        private TextBox materialGlow;
        private TextBox materialClimateColorMap;
        private TextBox materialSeasonColorMap;
        private ComboBox materialRenderPass;
        private TextBox materialZOffset;
        private CheckBox materialWaterWave;
        private CheckBox materialReflective;

        static MaterialPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaterialPanel), new FrameworkPropertyMetadata(typeof(MaterialPanel)));
        }

        public MaterialPanel(MaterialProperties material, ObservableCollection<string> textureKeys)
        {
            this.material = material;
            this.textureKeys = textureKeys;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            materialName = GetTemplateChild("materialName") as TextBox;
            materialTexture = GetTemplateChild("materialTexture") as ComboBox;
            materialShade = GetTemplateChild("materialShade") as CheckBox;
            materialGlow = GetTemplateChild("materialGlow") as TextBox;
            materialClimateColorMap = GetTemplateChild("materialClimateColorMap") as TextBox;
            materialSeasonColorMap = GetTemplateChild("materialSeasonColorMap") as TextBox;
            materialRenderPass = GetTemplateChild("materialRenderPass") as ComboBox;
            materialZOffset = GetTemplateChild("materialZOffset") as TextBox;
            materialWaterWave = GetTemplateChild("materialWaterWave") as CheckBox;
            materialReflective = GetTemplateChild("materialReflective") as CheckBox;

            materialName.Text = material.name ?? "";
            materialTexture.ItemsSource = new ReadOnlyCollection<string>(textureKeys);
            materialTexture.SelectedIndex = string.IsNullOrEmpty(material.texture) ? -1 : textureKeys.IndexOf(material.texture);
            materialShade.IsChecked = material.shade;
            ControlUtils.InitIntegerField(materialGlow);
            materialGlow.Text = material.glow.ToString();
            materialClimateColorMap.Text = material.climateColorMap ?? "";
            materialSeasonColorMap.Text = material.seasonColorMap ?? "";
            materialRenderPass.SelectedIndex = material.renderPass;
            ControlUtils.InitIntegerField(materialZOffset);
            materialZOffset.Text = material.zOffset.ToString();
            materialWaterWave.IsChecked = material.waterWave;
            materialReflective.IsChecked = material.reflective;

            materialName.TextChanged += OnNameChanged;
            materialTexture.SelectionChanged += OnTextureChanged;
            textureKeys.CollectionChanged += OnTextureListChanged;
            materialShade.Checked += OnShadeChanged;
            materialShade.Unchecked += OnShadeChanged;
            materialGlow.TextChanged += OnGlowChanged;
            materialClimateColorMap.TextChanged += OnClimateColorMapChanged;
            materialSeasonColorMap.TextChanged += OnSeasonColorMapChanged;
            materialRenderPass.SelectionChanged += OnRenderPassChanged;
            materialZOffset.TextChanged += OnZOffsetChanged;
            materialWaterWave.Checked += OnWaterWaveChanged;
            materialWaterWave.Unchecked += OnWaterWaveChanged;
            materialReflective.Checked += OnReflectiveChanged;
            materialReflective.Unchecked += OnReflectiveChanged;
        }

        public void Dispose()
        {
            textureKeys.CollectionChanged -= OnTextureListChanged;
        }

        private void OnTextureListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if(e.OldStartingIndex == materialTexture.SelectedIndex)
                    {
                        materialTexture.SelectedIndex = -1;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if(materialTexture.SelectedIndex >= 0)
                    {
                        materialTexture.SelectedIndex = -1;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    int index = materialTexture.SelectedIndex;
                    if(e.OldStartingIndex == index || e.NewStartingIndex == index)
                    {
                        materialTexture.SelectedItem = textureKeys[index];
                    }
                    break;
            }
        }

        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            material.name = materialName.Text.Trim();
            onNameChanged?.Invoke();
        }

        private void OnTextureChanged(object sender, SelectionChangedEventArgs e)
        {
            if(materialTexture.SelectedIndex < 0)
            {
                material.texture = null;
            }
            else
            {
                material.texture = textureKeys[materialTexture.SelectedIndex];
            }
        }

        private void OnShadeChanged(object sender, RoutedEventArgs e)
        {
            material.shade = materialShade.IsChecked == true;
        }

        private void OnGlowChanged(object sender, TextChangedEventArgs e)
        {
            material.glow = ControlUtils.GetInteger(materialGlow);
        }

        private void OnClimateColorMapChanged(object sender, TextChangedEventArgs e)
        {
            material.climateColorMap = TextOrNull(materialClimateColorMap.Text);
        }

        private void OnSeasonColorMapChanged(object sender, TextChangedEventArgs e)
        {
            material.seasonColorMap = TextOrNull(materialSeasonColorMap.Text);
        }

        private void OnRenderPassChanged(object sender, SelectionChangedEventArgs e)
        {
            material.renderPass = (short)materialRenderPass.SelectedIndex;
        }

        private void OnZOffsetChanged(object sender, TextChangedEventArgs e)
        {
            material.zOffset = ControlUtils.GetShort(materialZOffset);
        }

        private void OnWaterWaveChanged(object sender, RoutedEventArgs e)
        {
            material.waterWave = materialWaterWave.IsChecked == true;
        }

        private void OnReflectiveChanged(object sender, RoutedEventArgs e)
        {
            material.reflective = materialReflective.IsChecked == true;
        }

        private static string TextOrNull(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
    }
}