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
    public partial class UserAgreementView : ContentPage
    {
        public UserAgreementView()
        {
            InitializeComponent();

            var htmlSource = new HtmlWebViewSource()
            {
                Html = FScruiser.XF.Properties.Resources.UserAgreement
            };
            webView.Source = htmlSource;
        }
    }
}