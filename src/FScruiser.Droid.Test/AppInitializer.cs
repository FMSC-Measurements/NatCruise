using System;
using System.Reflection;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            AppExtentions.ScreenShotFolder = Path.Combine(Path.GetTempPath(), "TestTemp", Assembly.GetExecutingAssembly().GetName().Name, "ScreenShots");

            if (platform == Platform.Android)
            {
                //Environment.SetEnvironmentVariable("ANDROID_HOME", "%ProgramFiles(x86)%\\Android\\android-sdk", EnvironmentVariableTarget.Process);
                //Environment.SetEnvironmentVariable("JAVA_HOME", "%ProgramFiles%\Android\Jdk\microsoft_dist_openjdk_1.8.0.25");

                var keystorePath = Path.GetFullPath("../../../../FScruiser.Droid/FMSC.keystore");
                var keystorePW = Environment.GetEnvironmentVariable("FScruiserKeystorePass");

                return ConfigureApp
                .Android
                .EnableLocalScreenshots()
                //.KeyStore(keystorePath, keystorePW, keystorePW, keyAlias: "FMSC") // if testing a signed release version the test runner needs to be signed with the same keystore
                .InstalledApp("com.FMSC.FScruiser")
                //.ApkFile("../../../FScruiser/FScruiser.Droid/bin/Release/FScruiser.Droid.apk")
                .StartApp(Xamarin.UITest.Configuration.AppDataMode.DoNotClear);

            }
            else
            {
                throw new NotSupportedException();
                //return ConfigureApp.iOS.StartApp();
            }
        }
    }
}