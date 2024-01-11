using Backpack.Maui.Controls;
using Backpack.Maui.Handlers.ValuePicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backpack.Maui;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder UseBackpack(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(ConfigrueHandlers);

        return builder;
    }

    private static void ConfigrueHandlers(IMauiHandlersCollection handlers)
    {
        handlers.AddValuePicker();
    }

    public static IMauiHandlersCollection AddValuePicker(this IMauiHandlersCollection handlers) 
    {
        handlers.AddHandler<ValuePicker, ValuePickerHandler>();

        return handlers;
    }
}

