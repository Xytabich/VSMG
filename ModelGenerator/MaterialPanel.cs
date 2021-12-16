using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ModelGenerator
{
    public class MaterialPanel : Control
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
            materialTexture.IsSynchronizedWithCurrentItem = true;
            materialTexture.ItemsSource = textureKeys;
            materialTexture.SelectedIndex = string.IsNullOrEmpty(material.texture) ? -1 : textureKeys.IndexOf(material.texture);
            materialShade.IsChecked = material.shade;
            materialGlow.InitIntegerField();
            materialGlow.SetInteger(material.glow);
            materialClimateColorMap.Text = material.climateColorMap ?? "";
            materialSeasonColorMap.Text = material.seasonColorMap ?? "";
            materialRenderPass.SelectedIndex = material.renderPass;
            materialZOffset.InitIntegerField();
            materialZOffset.SetInteger(material.zOffset);
            materialWaterWave.IsChecked = material.waterWave;
            materialReflective.IsChecked = material.reflective;

            materialName.TextChanged += OnNameChanged;
            materialTexture.SelectionChanged += OnTextureChanged;
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
            material.glow = materialGlow.GetInteger();
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
            material.zOffset = materialZOffset.GetShort();
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