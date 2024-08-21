using NatCruise.Async;

namespace FScruiser.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PrivacyPolicyView : ContentPage
{
    public PrivacyPolicyView()
    {
        InitializeComponent();
    }

    protected async Task LoadPrivacyPolicy()
    {
        var stream = await FileSystem.OpenAppPackageFileAsync("PrivacyPolicy.htm");
        using var reader = new StreamReader(stream);

        var contents = reader.ReadToEnd();
        var htmlSource = new HtmlWebViewSource()
        {
            Html = contents
        };
        _webView.Source = htmlSource;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadPrivacyPolicy().FireAndForget();
    }
}