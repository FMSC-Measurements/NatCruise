using Microsoft.Maui.Controls.Internals;

using IElement = Microsoft.Maui.IElement;

namespace Backpack.Maui.Controls
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

        static IEnumerable<Element> GetParentsPath(this Element self)
        {
            Element current = self;

            while (!IsApplicationOrNull(current.RealParent))
            {
                current = current.RealParent;
                yield return current;
            }
        }

        public static Page? GetPage(this IView @this)
        {
            for (Element element = @this.Parent as Element; element != null; element = element.RealParent)
            {
                if (element is Page page1)
                    return page1;
            }
            return (Page?)null;
        }

        static bool IsApplicationOrNull(object? element) =>
            element == null || element is IApplication;
    }
}
