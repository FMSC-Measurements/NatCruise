using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;



namespace FScruiser.Droid.Test.Screens
{

    public abstract class ScreenBase
    {
        protected readonly IApp App;
        public Query page { get; protected set; }

        public Query alertDialog = x => x.Class("AlertDialogLayout");

        protected ScreenBase(IApp app)
        {
            App = app;
        }

        public void VerifyScreenDisplayed()
        {

            var pageResult = App.Query(page);
            page.Should().NotBeNull();
        }
    }
}
