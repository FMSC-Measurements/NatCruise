using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;

namespace FScruiser.Droid.Test.Screens
{
    public class CruiseListScreen : ScreenBase
    {
        public CruiseListScreen(IApp app) : base(app)
        {
        }

        public readonly Query page = x => x.Marked("Cruise Selection Page");

        public readonly Query cruiseList = x => x.Marked("Cruise List");

        public readonly Query openButton = x => x.Marked("Open Cruise");

        public IEnumerable<string> GetCruises()
        {
            var cruiseListElmt = App.Query(cruiseList);
            var cruiseListItems = App.Query(x => cruiseList(x).Descendant("AppCompatTextView"));

            return App.Query(x => cruiseList(x).Descendant("AppCompatTextView"))
                .Select(x => x.Text)
                .ToArray();
        }

        public void OpenCruise(string cruiseTitle)
        {
            App.Tap(x => cruiseList(x).Descendant("AppCompatTextView").Text(cruiseTitle));
            App.Tap(openButton);
        }
    }
}
