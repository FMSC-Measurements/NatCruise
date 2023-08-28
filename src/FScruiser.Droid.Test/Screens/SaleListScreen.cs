using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;

namespace FScruiser.Droid.Test.Screens
{
    public class SaleListScreen : ScreenBase
    {

        public readonly Query saleList = x => x.Marked("Sale List");

        public SaleListScreen(IApp app) : base(app)
        {
            page = x => x.Marked("Select Sale Page");
        }

        public IEnumerable<string> GetSales()
        {
            return App.Query(x => saleList(x).Descendant("AppCompatTextView")).Select(x => x.Text).ToArray();
        }

        public void SelectSale(string saleTitle)
        {
            var saleItem = App.Query();
            App.Tap(x => saleList(x).Descendant("AppCompatTextView").Text(saleTitle));
        }

    }
}
