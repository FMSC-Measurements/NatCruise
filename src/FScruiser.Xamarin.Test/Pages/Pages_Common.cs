using FluentAssertions;
using FScruiser.XF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.Pages
{
    public class Pages_Common : TestBase
    {
        public Pages_Common(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<object[]> PageTypes
        {
            get
            {
                var assembly = Assembly.GetAssembly(typeof(MainPage));

                var pageTypes = assembly.GetTypes()
                .Where(t => t.Namespace == "FScruiser.XF.Pages" && t.IsSubclassOf(typeof(Xamarin.Forms.Page)))
                .Select(x => new object[] { x });

                return  pageTypes;
            }
        }

        /// <summary>
        /// verify that all page classes initialize and there are no silly mistakes in the xaml
        /// </summary>
        [Theory]
        [MemberData(nameof(PageTypes))]
        public void CreatePage(Type pageType)
        {
            if(pageType.IsAbstract) { return; }

            var page = Activator.CreateInstance(pageType) as Xamarin.Forms.Page;

            page.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(PageTypes))]
        public void ResolveViewModel(Type pageType)
        {
            if (pageType.IsAbstract) { return; }

            //get our page
            var page = Activator.CreateInstance(pageType) as Xamarin.Forms.Page;

            //call this method that causes Prism to resolve the viewmodel for our view
            Prism.Mvvm.ViewModelLocationProvider.AutoWireViewModelChanged(page, (p, vm) =>
            {
                p.Should().NotBeNull();
                vm.Should().NotBeNull();

                Output.WriteLine($"ViewModelType:{vm.GetType().Name}");

                ((Page)page).BindingContext = vm;
            });
        }
    }
}