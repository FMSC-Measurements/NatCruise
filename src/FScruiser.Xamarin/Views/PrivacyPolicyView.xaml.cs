using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivacyPolicyView : ContentPage
    {
        public PrivacyPolicyView()
        {
            InitializeComponent();

            var htmlSource = new HtmlWebViewSource()
            {
                Html = Properties.Resources.PrivacyPolicy
            };
            webView.Source = htmlSource;
        }
    }
}