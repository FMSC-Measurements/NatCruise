using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Util
{
    public static class FrameWorkElementExtentions
    {
        public static Window GetWindow(this FrameworkElement element)
        {

            do
            {
                if (element is Window window)
                {
                    return window;
                }
                element = element?.Parent as FrameworkElement;
            } while (element != null);

            return null;
        }
    }
}
