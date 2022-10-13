using System;
using Xamarin.Forms;

namespace FScruiser.XF.Util
{
    public static class ElementExtentions
    {
        public static T GetAncestor<T>(this Element element) where T : Element
        {
            if (element is null) { throw new System.ArgumentNullException(nameof(element)); }

            var type = typeof(T);
            while (element != null)
            {
                if (element is T found) { return found; }
                element = element.Parent;
            }
            return null;
        }

        public static Element GetAncestor(this Element element, Type type)
        {
            if (element is null) { throw new System.ArgumentNullException(nameof(element)); }

            while (element != null)
            {
                if (element.GetType() == type) { return element; }
                element = element.Parent;
            }
            return null;
        }
    }
}