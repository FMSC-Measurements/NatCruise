using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedbackPage : ContentPage
    {
        public FeedbackPage()
        {
            InitializeComponent();

            _submitButton.Clicked += _submitButton_Clicked;
            _feedbackEntry.TextChanged += _feedbackEntry_TextChanged;

            _feedbackEntry_TextChanged(null, null);
        }

        private void _feedbackEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var feedback = _feedbackEntry.Text;
            _submitButton.IsEnabled = !string.IsNullOrWhiteSpace(feedback);
        }

        private void _submitButton_Clicked(object sender, EventArgs e)
        {
            var feedback = _feedbackEntry.Text;
            if (!string.IsNullOrWhiteSpace(feedback))
            {
                var email = _emailEntry.Text;
                var name = _nameEntry.Text;

                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Feedback"
                    , new Dictionary<string, string>() {
                        { "feedback_email", email }
                        ,{ "feedback_name", name }
                        ,{ "feedback_comment", feedback }
                    });

                _feedbackEntry.Text = null;

                base.SendBackButtonPressed();
            }
        }
    }
}