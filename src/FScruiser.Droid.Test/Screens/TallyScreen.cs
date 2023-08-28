using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace FScruiser.Droid.Test.Screens
{
    public class TallyScreen : ScreenBase
    {
        public readonly Query tallyFeed = x => x.Marked("Tally Feed");
        public readonly Query treeItem = x => x.Marked("Tally Feed").Descendant().Marked("Tree Item");
        public readonly Query tallyEntryItem = x => x.Marked("Tally Feed").Descendant().Marked("Tally Entry Item");

        public readonly Query tallyPopulationList = x => x.Marked("Tally Population List");
        public readonly Query tallyPopulationItem = x => x.Marked("Tally Population Item");
        public readonly Query tallyMenuButton = x => x.Marked("Tally Menu");

        public TallyScreen(IApp app) : base(app)
        {
            page = x => x.Marked("Tally Page");
        }

        public int GetTallyEntryCount()
        {
            return App.Query(x => tallyFeed(x).Child()).Length;
        }

        public IEnumerable<AppResult> GetTallyPopulationItems()
        {
            return App.Query(tallyPopulationItem);
        }
    }
}
