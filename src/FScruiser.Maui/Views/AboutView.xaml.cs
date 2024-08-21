using FScruiser.Maui.Controls;
using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutView : BasePage
	{
		public AboutView ()
		{
			InitializeComponent ();
		}

		public AboutView(AboutViewModel vm) : this()
		{
			BindingContext = vm;
		}
	}
}