using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public static class ElementExtentions
    {
        public static Page GetPage(this Element @this)
        {
            for (Element element = @this.Parent; element != null; element = element.RealParent)
            {
                if (element is Page page1)
                    return page1;
            }
            return (Page)null;
        }
    }
}