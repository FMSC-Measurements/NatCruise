using AndroidX.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpo.Logger;
using FScruiser.Maui.Services;
using FScruiser.Maui.Views;
using Microsoft.Extensions.Logging;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatCruise.Async;
using NatCruise.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public class NavItemModel
        {
            public string Title { get; set; }
            public Type ViewType { get; set; }

            public Func<ICruiseNavigationService, Task> NavAction { get; set; }

        }

        [ObservableProperty]
        private Cruise? _selectedCruise;

        [ObservableProperty]
        private IReadOnlyCollection<CuttingUnit> _cuttingUnits;

        [ObservableProperty]
        private CuttingUnit? _selectedCuttingUnit;

        [ObservableProperty]
        private IReadOnlyCollection<NavItemModel> _navItems;

        [ObservableProperty]
        private NavItemModel? _selectedNavItem;
        private ICommand _showSelectSaleCommand;

        public ICommand ShowSelectSaleCommand => _showSelectSaleCommand ??= new Command(() => NavigationService.ShowSaleSelect().FireAndForget());

        public IServiceProvider Services { get; }
        public ICruiseNavigationService NavigationService { get; }
        public IDataContextService DataContext { get; }



        [RelayCommand]
        public void Navigate()
        {
            if (SelectedNavItem != null)
            {
                Navigate(SelectedNavItem);
            }

        }

        partial void OnSelectedCuttingUnitChanged(CuttingUnit? value)
        {
            RefreshNavItems();
        }

        public void Navigate(NavItemModel navItem)
        {
            navItem.NavAction.Invoke(NavigationService).FireAndForget();
        }




        public MainViewModel(IServiceProvider services, ILogger<MainViewModel> log, ICruiseNavigationService navigationService, IDataContextService dataContext)
        {
            Services = services;
            NavigationService = navigationService;
            DataContext = dataContext;

            dataContext.CruiseChanged += DataContext_CruiseChanged;
            DataContext_CruiseChanged(null, EventArgs.Empty);
        }

        public override void Load()
        {
            base.Load();

            RefreshNavItems();
        }



        private void DataContext_CruiseChanged(object? sender, EventArgs e)
        {
            var cruiseID = DataContext.CruiseID;
            if (cruiseID != null)
            {
                SelectedCruise = Services.GetRequiredService<ISaleDataservice>().GetCruise(cruiseID);
                SelectedCuttingUnit = null;
                CuttingUnits = Services.GetRequiredService<ICuttingUnitDataservice>().GetCuttingUnits();
            }
            else
            {
                SelectedCruise = null;
                SelectedCuttingUnit = null;
                CuttingUnits = ReadOnlyCollection<CuttingUnit>.Empty;
            }
        }

        public void RefreshNavItems()
        {
            

            var navList = new List<NavItemModel>();
            if (SelectedCruise != null)
            {
                navList.Add(new NavItemModel { Title = "Sale", NavAction = n => ShowSale() });
                navList.Add(new NavItemModel { Title = "Strata", NavAction = n => ShowStrata() });
                navList.Add(new NavItemModel { Title = "Audit Rules", NavAction = n => ShowAuditRules() });
            }

            var selectedCuttingUnit = SelectedCuttingUnit;
            if (selectedCuttingUnit != null)
            {

                var selectedUnitInfo = Services.GetRequiredService<ICuttingUnitDataservice>().GetCuttingUnitStrataSummary(selectedCuttingUnit.CuttingUnitCode);

                if (selectedUnitInfo.HasTreeStrata)
                {
                    navList.Add(new NavItemModel { Title = "Trees", NavAction = n => ShowTrees() });
                    navList.Add(new NavItemModel { Title = "Tally", NavAction = n => ShowTally() });
                }
                if (selectedUnitInfo.HasPlotStrata)
                {
                    navList.Add(new NavItemModel { Title = "Plot Trees", NavAction = n => ShowPlotTrees() });
                    navList.Add(new NavItemModel { Title = "Plots", NavAction = n => ShowPlots() });
                }

                navList.Add(new NavItemModel { Title = "Cutting Unit", NavAction = n => ShowCuttingUnit() });


            }
            else
            {
                navList.Add(new NavItemModel { Title = "Utilities", NavAction = (n) => n.ShowUtilities() });
                navList.Add(new NavItemModel { Title = "Cruisers", NavAction = (n) => n.ShowManageCruisers() });
                navList.Add(new NavItemModel { Title = "Settings", NavAction = (n) => n.ShowSettings() });
                navList.Add(new NavItemModel { Title = "About", NavAction = (n) => n.ShowAbout() });
            }

            NavItems = navList.ToArray();

        }

        public Task ShowTrees()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowTreeList(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
        }

        public Task ShowTally()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowTally(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
        }

        public Task ShowPlotTrees()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowPlotTreeList(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
        }

        public Task ShowPlots()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowPlotList(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
        }

        public Task ShowCuttingUnit()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowCuttingUnitInfo(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
        }

        public Task ShowSale()
        {
            return NavigationService.ShowSale(SelectedCruise.CruiseID);
        }

        public Task ShowStrata()
        {
            return NavigationService.ShowStrata();
        }

        public Task ShowAuditRules()
        {
            return NavigationService.ShowTreeAuditRules();
        }

        public Task ShowCruisers()
        {
            return NavigationService.ShowManageCruisers();
        }

        public Task ShowUtilities()
        {
            return NavigationService.ShowUtilities();
        }

        [RelayCommand]
        public Task ShowTestView()
        {
            return (NavigationService as MauiNavigationService)?.ShowView<TestDialogServiceView>() ?? Task.CompletedTask;
        }
    }
}
