﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FScruiser.XF.Controls
{
    public static class LayoutExtensions
    {
        public static IReadOnlyList<Element> GetChildren(this ILayoutController source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.Children;
        }
    }
}