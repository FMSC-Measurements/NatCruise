using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            // TODO: If the Android app being tested is included in the solution then open
            // the Unit Tests window, right click Test Apps, select Add App Project
            // and select the app projects that should be tested.
            app = ConfigureApp
                .Android
                .EnableLocalScreenshots()
                .ApkFile("../../../FScruiser/FScruiser.Droid/bin/Release/FScruiser.Droid.apk")
                .StartApp();
        }

        [Test]
        public void AppLaunches()
        {
            app.WaitForElement(c => c.Class("ContenPage"));

            app.Screenshot("First screen.");
        }
    }
}