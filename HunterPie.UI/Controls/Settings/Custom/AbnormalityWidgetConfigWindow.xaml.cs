﻿using HunterPie.Core.Client.Configuration.Overlay;
using HunterPie.UI.Controls.Settings.Custom.Abnormality;
using HunterPie.UI.Controls.Settings.ViewModel;
using HunterPie.UI.Settings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using HunterPie.UI.Controls.TextBox.Events;
using System.Text.RegularExpressions;

namespace HunterPie.UI.Controls.Settings.Custom
{
    /// <summary>
    /// Interaction logic for AbnormalityWidgetConfig.xaml
    /// </summary>
    public partial class AbnormalityWidgetConfigWindow : Window, INotifyPropertyChanged
    {
        private AbnormalityCollectionViewModel _selectedElement;

        public ObservableCollection<AbnormalityCollectionViewModel> Collections { get; } = new();
        public ObservableCollection<ISettingElementType> Elements { get; } = new();

        public AbnormalityCollectionViewModel SelectedCollection
        {
            get => _selectedElement;
            set
            {
                if (value != _selectedElement)
                {
                    _selectedElement = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCollection)));
                }
            }
        }

        public readonly AbnormalityWidgetConfig Config;

        public event PropertyChangedEventHandler PropertyChanged;

        public AbnormalityWidgetConfigWindow(AbnormalityWidgetConfig config)
        {
            Config = config;
            DataContext = config;
            InitializeComponent();

            BuildVisualConfig();
            LoadAbnormalities();
            HookEvents();
        }

        private void HookEvents()
        {
            Config.Game.PropertyChanged += OnGameTypeChanged;
        }

        private void UnhookEvents()
        {
            Config.Game.PropertyChanged -= OnGameTypeChanged;
        }

        private void BuildVisualConfig()
        {
            foreach (ISettingElementType element in VisualConverterManager.BuildSubElements(Config))
                Elements.Add(element);
        }

        private void OnGameTypeChanged(object sender, PropertyChangedEventArgs e) => LoadAbnormalities();

        private void LoadAbnormalities()
        {
            Collections.Clear();

            var collections = AbnormalitiesViewHelper.GetViewModelsBy(Config);

            foreach (AbnormalityCollectionViewModel coll in collections)
                Collections.Add(coll);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            UnhookEvents();
            Config.AllowedAbnormalities.Clear();

            foreach (AbnormalityCollectionViewModel collection in Collections)
                foreach (var abnorm in collection.Abnormalities.Where(a => a.IsEnabled))
                    Config.AllowedAbnormalities.Add(abnorm.Id);
        }

        private void OnSearchTextChanged(object sender, SearchTextChangedEventArgs e)
        {
            if (SelectedCollection is not null)
            {
                foreach (AbnormalityViewModel vm in SelectedCollection.Abnormalities)
                {
                    vm.IsMatch = string.IsNullOrEmpty(e.Text) || Regex.IsMatch(vm.Name, e.Text, RegexOptions.IgnoreCase);
                }
            }
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            if (SelectedCollection is not null)
            {
                foreach (AbnormalityViewModel vm in SelectedCollection.Abnormalities)
                    vm.IsEnabled = true;
            }
        }
    }
}
