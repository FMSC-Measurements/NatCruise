using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NatCruise.Wpf.Controls
{
    public static class FlowDocumentReaderHelper
    {
        public static string GetRtfText(DependencyObject obj)
        {
            return (string)obj.GetValue(RtfTextProperty);
        }

        public static void SetRtfText(DependencyObject obj, string value)
        {
            obj.SetValue(RtfTextProperty, value);
        }


        public static readonly DependencyProperty RtfTextProperty = DependencyProperty.RegisterAttached(
            "RtfText",
            typeof(string),
            typeof(FlowDocumentReaderHelper),
            new UIPropertyMetadata(propertyChangedCallback: RtfTextChanged));

        private static void RtfTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fdr = d as FlowDocumentReader;
            var rtfText = e.NewValue as string;

            if(fdr != null && rtfText != null)
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(rtfText);

                var flowDoc = new FlowDocument();
                var textRange = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
                textRange.Load(stream, DataFormats.Rtf);

                fdr.Document = flowDoc;

            }
        }
    }
}
