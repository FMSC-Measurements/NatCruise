﻿using FScruiser.Maui.Controls;
using FScruiser.Maui.ViewModels;

namespace FScruiser.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LimitingDistanceView : BasePage
    {
        protected LimitingDistanceView()
        {
            InitializeComponent();

            //_treeInOutLabel.PropertyChanged += _treeInOutLabel_PropertyChanged;
            //_bafOrFpsEntry.Completed += (s, e) => _dbhEntry.Focus();
            //_dbhEntry.Completed += (sender, e) => _slopePctEntry.Focus();
            //_slopePctEntry.Completed += (s, e) => _slopeDistanceEntry.Focus();
            //_slopeDistanceEntry.Completed += (s, e) => _azimuthEntry.Focus();
        }

        public LimitingDistanceView(LimitingDistanceViewModel viewModel) : this()
        {
            BindingContext = viewModel;
        }


        //private void _treeInOutLabel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    var label = sender as Label;
        //    if (label == null) { return; }

        //    if (e.PropertyName == nameof(Entry.Text))
        //    {
        //        var text = label.Text;
        //        switch (text)
        //        {
        //            case "Tree is " + CalculateLimitingDistance.TREE_STATUS_IN:
        //                {
        //                    label.BackgroundColor = Color.Green; break;
        //                }
        //            case "Tree is " + CalculateLimitingDistance.TREE_STATUS_OUT:
        //                {
        //                    label.BackgroundColor = Color.Red; break;
        //                }
        //            default:
        //                {
        //                    label.BackgroundColor = Color.White; break;
        //                }
        //        }
        //    }
        //}
    }
}