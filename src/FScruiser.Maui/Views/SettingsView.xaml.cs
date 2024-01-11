using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FScruiser.Maui.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsView : ContentPage
	{
        private bool _courcingSliderValue;

        public SettingsView ()
		{
			InitializeComponent ();
		}

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
			if(_courcingSliderValue == true) { return; }
			try
			{
				_courcingSliderValue = true;
				var slider = (Slider)sender;
				var value = e.NewValue;
				slider.Value = Math.Round(value, 1);
			}
			finally { _courcingSliderValue = false; }
        }
    }
}