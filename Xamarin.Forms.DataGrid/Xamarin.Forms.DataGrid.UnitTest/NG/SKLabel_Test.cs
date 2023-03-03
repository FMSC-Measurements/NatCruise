using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.DataGrid.UnitTest.NG
{
    [TestClass]
    public class SKLabel_Test
    {
        [TestMethod]
        public void SplitLines_Test()
        {
            var fontSize = 12;
            var maxWidth = 300.0d;
            var scalingFactor = 1.0d;

            var skPaint = new SKPaint
            {
                IsAntialias = true,
                IsAutohinted = true,
                SubpixelText = true,
                Color = SKColor.Empty,
                Typeface = Font.Default.ToSKTypeface(),
                TextSize = (float)(fontSize * scalingFactor)
            };

            var lines = SKLabel.SplitLines("someting", skPaint, maxWidth);

            Assert.IsNotNull(lines);
        }
    }
}
