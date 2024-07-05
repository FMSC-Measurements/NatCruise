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
            RefreshNavItems();
        }

        public Task ShowTrees()
        {
            if (SelectedCuttingUnit != null)
            {
                return NavigationService.ShowTreeList(SelectedCuttingUnit.CuttingUnitCode);
            }
            return Task.CompletedTask;
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
            var selectedCuttingUnit = SelectedCuttingUnit;
            if (selectedCuttingUnit != null)
            {
                NavItems = new[]
                {
                    new NavItemModel { Title = "Trees", NavAction= n => ShowTrees() },

                    new NavItemModel { Title = "Utilities", NavAction = (n) => n.ShowUtilities() },
                    new NavItemModel { Title = "Settings", NavAction = (n) => n.ShowSettings() },
                    new NavItemModel { Title = "About", NavAction= (n) => n.ShowAbout()},
                };
            }
            else
            {
                NavItems = new[]
                {
                    new NavItemModel { Title = "Utilities", NavAction = (n) => n.ShowUtilities() },
                    new NavItemModel { Title = "Settings", NavAction = (n) => n.ShowSettings() },
                    new NavItemModel { Title = "About", NavAction= (n) => n.ShowAbout()},
                };
            }

        }
    }
}
