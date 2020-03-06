using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskKpiPage : DialogPage
    {
        public AskKpiPage()
        {
            InitializeComponent();

            foreach (var btn in _grid.Children.OfType<Button>())
            {
                btn.Clicked += Btn_Clicked;
            }
        }


        public int? MinKPI { get; set; }

        public int? MaxKPI { get; set; }

        public int MaxValueLength => 6;

        public string DisplayValue
        {
            get { return _kpiLabel.Text; }
            protected set { _kpiLabel.Text = value; }
        }


        private void Btn_Clicked(object sender, EventArgs e)
        {
            if (sender == null) { return; }
            var btn = (Button)sender;

            var btnText = btn.Text;

            OnButtonPress(btnText);
        }

        private void OnButtonPress(string buttonValue)
        {
            switch (buttonValue)
            {
                case "C": { DisplayValue = ""; break; }
                case "OK": { Close(DialogResult.OK); break; }
                case "Cancel": { Close(DialogResult.Cancel); break; }
                case "STM":
                    {
                        DisplayValue = "";
                        Close(DialogResult.OK);
                        break;
                    }
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                    {
                        DisplayValue = DisplayValue + buttonValue;
                        break;
                    }
            }
        }

        public int? GetUserEnteredValue()
        {
            var displayValue = DisplayValue;
            if (displayValue == "STM")
            {
                return -1;
            }

            if (int.TryParse(DisplayValue, out int value))
            { return value; }
            else { return null; }
        }

        protected override bool OnClosing(DialogResult result, out object output)
        {
            if (result == DialogResult.OK)
            {
                output = CheckInput(DisplayValue, MinKPI, MaxKPI, out var errorMessage);
                if (output != null)
                {
                    return true;
                }
                else
                {
                    DisplayAlert("", errorMessage, "OK");
                    return false;
                }
            }
            else
            {
                output = null;
                return true;
            }
        }

        public static AskKPIResult CheckInput(string displayValue, int? minKPI, int? maxKPI, out string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(displayValue))
            {
                errorMessage = "No Value Entered";
                return null;
            }
            else if (displayValue == "STM")
            {
                errorMessage = null;
                return new AskKPIResult { IsSTM = true, DialogResult = Pages.DialogResult.OK };
            }
            else if (int.TryParse(displayValue, out int i_value))
            {
                if (minKPI != null && i_value < minKPI.Value && minKPI.Value > 0)
                {
                    errorMessage = $"Value Must be Greater or Equal to {minKPI}";
                    return null;
                }
                else if (maxKPI != null && maxKPI > 0 && (maxKPI.Value > (minKPI ?? 0)) && i_value > maxKPI.Value)
                {
                    errorMessage = $"Value Must be Less Than or Equal to {maxKPI}";
                    return null;
                }
                else
                {
                    errorMessage = null;
                    return new AskKPIResult { KPI = i_value, DialogResult = Pages.DialogResult.OK };
                }
            }
            else
            {
                errorMessage = "Invalid Value";
                return null;
            }
        }
    }

    public class AskKPIResult
    {
        public DialogResult DialogResult { get; set; }
        public bool IsSTM { get; set; }
        public int KPI { get; set; }
    }
}