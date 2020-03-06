using FScruiser.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClickerTreeCountEntryDialog : DialogPage
    {
        public ClickerTreeCountEntryDialog()
        {
            Data = new AskTreeCountResult();
            InitializeComponent();
        }

        public ClickerTreeCountEntryDialog(int? defaultTreeCount) : this()
        {
            Data.TreeCount = defaultTreeCount;
            _treeCountEntry.Text = defaultTreeCount.ToString();
            //_cruiserPicker.ItemsSource = cruisers;
        }

        public AskTreeCountResult Data
        {
            get => BindingContext as AskTreeCountResult;
            set => BindingContext = value;
        }

        protected override bool OnClosing(DialogResult result, out object output)
        {
            if (result == DialogResult.Cancel)
            {
                output = null;
                return true;
            }
            else
            {
                var data = Data;
                var treeCount = data.TreeCount;
                //var cruiser = data.Cruiser;
                if (treeCount == null)
                {
                    DisplayAlert("", "No Tree Count Entered", "OK");
                    output = null;
                    return false;
                }
                else if (treeCount <= 0)
                {
                    DisplayAlert("", "Tree Count Needs to be a Positive Value", "OK");
                    output = null;
                    return false;
                }
                else
                {
                    output = data;
                    return true;
                }
            }
        }

        private void _treeCountEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Data.TreeCount = (int.TryParse(_treeCountEntry.Text, out int treeCount)) ? (int?)treeCount : null;
        }

        //private void _cruiserPicker_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var cruiser = _cruiserPicker.SelectedItem as string;
        //    Data.Cruiser = cruiser;
        //}

        private void _okButton_Clicked(object sender, EventArgs e)
        {
            Close(DialogResult.OK);
        }

        private void _cancelButton_Clicked(object sender, EventArgs e)
        {
            Close(DialogResult.Cancel);
        }
    }
}