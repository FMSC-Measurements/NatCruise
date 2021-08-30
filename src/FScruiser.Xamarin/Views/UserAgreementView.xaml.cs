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
                Html = Properties.Resources.UserAgreement
            };
            webView.Source = htmlSource;
        }
    }
}