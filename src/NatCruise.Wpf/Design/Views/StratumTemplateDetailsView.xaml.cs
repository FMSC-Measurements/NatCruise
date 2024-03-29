﻿using NatCruise.Design.ViewModels;
using NatCruise.Models;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for StratumTemplateDetailsView.xaml
    /// </summary>
    public partial class StratumTemplateDetailsView : UserControl
    {
        public StratumTemplateDetailsView()
        {
            InitializeComponent();

            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var selectedStratumTemplate = (StratumTemplate)context.Value;
            (DataContext as StratumTemplateDetailsViewModel).StratumTemplate = selectedStratumTemplate;
        }
    }
}