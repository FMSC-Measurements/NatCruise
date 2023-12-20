using FScruiser.Maui.Services;
using FScruiser.Maui.ViewModels;
using FScruiser.Maui.Views;
using NatCruise.Data;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui
{
    internal static class AppBuilderExtensions
    {
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {

            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<ShellViewModel>();

            return builder;
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<AppShell>();
            services.AddSingleton<ISoundService, SoundService>();
            services.AddSingleton<ICruiseNavigationService, MauiNavigationService>();
            services.AddSingleton<INatCruiseDialogService, MauiDialogService>();
            services.AddSingleton<IDataserviceProvider>

            return builder;
        }
    }
}
