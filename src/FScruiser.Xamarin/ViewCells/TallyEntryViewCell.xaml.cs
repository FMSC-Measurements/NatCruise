using FScruiser.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TallyEntryViewCell : TallyEntryViewCell_Base
    {

        public TallyEntryViewCell()
        {
            InitializeComponent();

            _untallyButton.Clicked += _untallyButton_Clicked;
        }

        protected override View DrawrView => _drawer;

        protected void _untallyButton_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is TallyEntry tallyEntry && tallyEntry != null)
            {
                UntallyCommand?.Execute(tallyEntry.TallyLedgerID);
            }
        }
    }
}