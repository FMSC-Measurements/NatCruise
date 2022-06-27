using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test
{
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests()
        {
            platform = Platform.Android;
            app = AppInitializer.StartApp(platform);
        }


        [Fact]
        public void AppLaunches()
        {
            app.WaitForElement(c => c.Marked("Select Cruise"));

            //app.WaitForElement(c => c.Class("MainView"));

            app.Screenshot("First screen.");
        }
    }
}
