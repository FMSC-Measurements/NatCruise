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
    public partial class SampleStateManagmentOtherDevicesPage : ContentPage
    {
        public SampleStateManagmentOtherDevicesPage()
        {
            InitializeComponent();
            Prism.Mvvm.ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}