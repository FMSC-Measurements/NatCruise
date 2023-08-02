using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xunit.Abstractions;

namespace FScruiser.Droid.Test
{

    public static class AppExtentions
    {
        private static string _screenShotFolder;

        public static string ScreenShotFolder
        {
            get => _screenShotFolder;
            set
            {
                if (!Directory.Exists(value) && !string.IsNullOrEmpty(value))
                {
                    Directory.CreateDirectory(value);
                }
                _screenShotFolder = value;
            }
        }

        public static void ScreenshotEx(this IApp @this, ITestOutputHelper testLog, string title)
        {
            var temp = @this.Screenshot(title);
            var scheenShotPath = Path.Combine(ScreenShotFolder, title + ".png");
            temp.CopyTo(scheenShotPath);
            testLog.WriteLine(scheenShotPath);
        }
    }
}
