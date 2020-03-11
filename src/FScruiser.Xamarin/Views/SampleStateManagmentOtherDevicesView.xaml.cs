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
    public partial class SampleStateManagmentOtherDevicesView : ContentPage
    {
        public SampleStateManagmentOtherDevicesView()
        {
            InitializeComponent();
            Prism.Mvvm.ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}