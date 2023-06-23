using FScruiser.XF.Controls;
using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreeErrorEditView : InitializableContentPage
    {
        public TreeErrorEditView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var vm = BindingContext as TreeErrorEditViewModel;
            if (vm != null)
            {
                vm.Saved = HandleViewModelSaved;
            }
        }

        private void HandleViewModelSaved(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}