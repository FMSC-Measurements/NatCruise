using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FScruiser.XF.Behaviors
{
    public class SelectNextOnCompleatedBehavior : Behavior<Entry>
    {
        private const int MAX_SEARCH_DEPTH = 10;
        public Type ParentType { get; set; } = typeof(StackLayout);

        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);

            entry.Completed += Entry_Completed;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            var ancestor = FindAncestor(entry, ParentType, out var lastChild) as Layout;
            if (ancestor == null) return;
            var ancestorChildren = ancestor.Children;
            var lastChildIndex = ancestorChildren.IndexOf(lastChild);

            if (lastChildIndex >= ancestorChildren.Count - 1) return;

            var nextChildDownTheLine = ancestorChildren[lastChildIndex + 1] as VisualElement;
            if (nextChildDownTheLine == null) return;

            var nextInput = FindNextInputOrPicker(nextChildDownTheLine);
            nextInput?.Focus();
        }

        public static Element FindAncestor(Element element, Type parentType, out Element lastChild)
        {
            lastChild = null;
            var depth = 0;
            do
            {
                if (element == null) return null;
                depth++;
                var ancestor = element.Parent;
                if (ancestor != null && ancestor.GetType() == parentType)
                {
                    lastChild = element;
                    return ancestor;
                }

                element = ancestor;
            } while (depth < MAX_SEARCH_DEPTH);

            return null;
        }

        public static VisualElement FindNextInputOrPicker(VisualElement view)
        {
            if (view is InputView || view is Picker) return view;

            if (view is Layout layout)
            {
                foreach (var child in layout.Children.OfType<VisualElement>())
                {
                    if (child is InputView || child is Picker) return child;
                    else if (child is Layout childLayout)
                    {
                        return FindNextInputOrPicker(childLayout);
                    }
                }
            }
            return null;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);

            entry.Completed -= Entry_Completed;
        }
    }
}