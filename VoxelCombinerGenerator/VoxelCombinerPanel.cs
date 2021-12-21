using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VoxelCombinerGenerator
{
    public class VoxelCombinerPanel : Control
    {
        public event Action onAddClick;

        private Panel generatorList = null;
        private Button generatorAdd = null;
        private List<VoxelGeneratorSettingsPanel> generators = null;

        static VoxelCombinerPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoxelCombinerPanel), new FrameworkPropertyMetadata(typeof(VoxelCombinerPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            generatorList = GetTemplateChild("generatorList") as Panel;
            generatorAdd = GetTemplateChild("generatorAdd") as Button;

            generatorAdd.Click += OnAddGenerator;

            if(generators != null)
            {
                for(int i = 0; i < generators.Count; i++)
                {
                    generatorList.Children.Add(generators[i]);
                }
                generators = null;
            }
        }

        public void AddGenerator(VoxelGeneratorSettingsPanel panel)
        {
            if(generatorList == null)
            {
                if(generators == null) generators = new List<VoxelGeneratorSettingsPanel>();
                generators.Add(panel);
            }
            else
            {
                generatorList.Children.Add(panel);
            }
        }

        public void RemoveGenerator(VoxelGeneratorSettingsPanel panel)
        {
            if(generatorList == null)
            {
                if(generators != null) generators.Remove(panel);
            }
            else
            {
                generatorList.Children.Remove(panel);
            }
        }

        public void ShowAddMenu(IReadOnlyList<string> items, Action<int> onSelect)
        {
            var menu = new ContextMenu();
            for(int i = 0; i < items.Count; i++)
            {
                AddItemToMenu(menu, items[i], i, onSelect);
            }
            menu.Closed += ClearButtonContextMenu;
            generatorAdd.ContextMenu = menu;
            menu.IsOpen = true;
        }

        private void ClearButtonContextMenu(object sender, RoutedEventArgs e)
        {
            generatorAdd.ContextMenu = null;
        }

        private void AddItemToMenu(ContextMenu menu, string title, int index, Action<int> onClick)
        {
            var item = new MenuItem();
            item.Header = title;
            item.Click += (s, e) => onClick.Invoke(index);
            menu.Items.Add(item);
        }

        private void OnAddGenerator(object sender, RoutedEventArgs e)
        {
            onAddClick?.Invoke();
        }
    }
}