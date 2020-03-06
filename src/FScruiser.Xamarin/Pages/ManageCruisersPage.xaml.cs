using FScruiser.XF.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageCruisersPage : ContentPage
    {
        public ManageCruisersPage()
        {
            InitializeComponent();
            _addCruiserButton.Clicked += _addCruiserButton_Clicked;
            _addCruiserEntry.Completed += _addCruiserEntry_Completed;
        }

        private void AddCruiser()
        {
            var cruiser = _addCruiserEntry.Text;

            if (BindingContext is ManageCruisersViewModel vm && vm != null)
            {
                vm.AddCruiser(cruiser);
                _addCruiserEntry.Text = null;
            }
        }

        private void _addCruiserEntry_Completed(object sender, EventArgs e)
        {
            AddCruiser();
        }

        private void _addCruiserButton_Clicked(object sender, EventArgs e)
        {
            AddCruiser();
        }
    }
}