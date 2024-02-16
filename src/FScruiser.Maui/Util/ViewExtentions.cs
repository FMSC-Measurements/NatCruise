using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace FScruiser.Maui.Util
{
    public static class ViewExtentions
    {
        internal static IMauiContext? FindMauiContext(this Element element, bool fallbackToAppMauiContext = false)
        {
            if (element is IElement fe && fe.Handler?.MauiContext != null)
                return fe.Handler.MauiContext;

            foreach (var parent in element.GetParentsPath())
            {
                if (parent is IElement parentView && parentView.Handler?.MauiContext != null)
                    return parentView.Handler.MauiContext;
            }

            return fallbackToAppMauiContext ? Application.Current?.FindMauiContext() : default;
        }

        internal static IEnumerable<Element> GetParentsPath(this Element self)
        {
            Element current = self;

            while (!IsApplicationOrNull(current.RealParent))
            {
                current = current.RealParent;
                yield return current;
            }

            bool IsApplicationOrNull(object? element) =>
            element == null || element is IApplication;
        }
    }
}
