using FScruiser.Logic;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LimitingDistancePage : ContentPage
    {
        public LimitingDistancePage()
        {
            InitializeComponent();

            _treeInOutLabel.PropertyChanged += _treeInOutLabel_PropertyChanged;
            _bafOrFpsEntry.Completed += (s,e) => _dbhEntry.Focus();
            _dbhEntry.Completed += (sender, e) => _slopePctEntry.Focus();
            _slopePctEntry.Completed += (s, e) => _slopeDistanceEntry.Focus();
            _slopeDistanceEntry.Completed += (s, e) => _azimuthEntry.Focus();
        }



        private void _treeInOutLabel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var label = sender as Label;
            if(label == null) { return; }

            if(e.PropertyName == nameof(Entry.Text))
            {
                var text = label.Text;
                switch(text)
                {
                    case "Tree is " + CalculateLimitingDistance.TREE_STATUS_IN:
                        {
                            label.BackgroundColor = Color.Green; break;
                        }
                    case "Tree is " + CalculateLimitingDistance.TREE_STATUS_OUT:
                        {
                            label.BackgroundColor = Color.Red; break;
                        }
                    default:
                        {
                            label.BackgroundColor = Color.White; break;
                        }
                }
            }
        }
    }
}