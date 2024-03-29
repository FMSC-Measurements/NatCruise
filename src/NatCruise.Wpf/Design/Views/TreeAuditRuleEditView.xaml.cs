﻿using NatCruise.Design.ViewModels;
using NatCruise.Models;
using NatCruise.MVVM.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace NatCruise.Design.Views
{
    /// <summary>
    /// Interaction logic for TreeAuditSelectorsView.xaml
    /// </summary>
    public partial class TreeAuditRuleEditView : UserControl
    {
        public TreeAuditRuleEditView()
        {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += Context_PropertyChanged;
        }

        private void Context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var treeAuditRule = (TreeAuditRule)context.Value;
            (DataContext as TreeAuditRuleEditViewModel).TreeAuditRule = treeAuditRule;
        }
    }
}