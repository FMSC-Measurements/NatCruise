using Xamarin.UITest;

namespace FScruiser.Droid.Test
{
    public class AppFixture : IDisposable
    {
        public IApp App { get; }
        public Platform Platform = Platform.Android;

        public AppFixture()
        {
            var app = App = AppInitializer.StartApp(Platform);
        }

        public void Dispose()
        {
            App.Repl();
        }
    }
}