using FScruiser.XF.Constants;
using NatCruise.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public class XamarinNavigationService : ICruiseNavigationService
    {
        ILoggingService Log { get; }

        public XamarinNavigationService(INavigationService navigationService, ILoggingService loggingService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            Log = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        INavigationService NavigationService { get; }

        public Task GoBackAsync()
        {
            return NavigationService.GoBackAsync();
        }

        public Task<INavigationResult> NavigateAsync(string path, NavigationParameters navparams)
        {
            return NavigationService.NavigateAsync(path, navparams);
        }

        public Task ShowManageCruisers()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Cruisers");
        }

        public Task ShowCruiseLandingLayout()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Blank");
        }

        public Task ShowCruiseSelect(string saleID)
        {
            return NavigationService.NavigateAsync("CruiseSelect",
                new NavigationParameters($"{NavParams.SaleID}={saleID}"));
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            return NavigationService.NavigateAsync("/Main/Navigation/CuttingUnitInfo",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }


        public Task ShowCuttingUnitList()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/CuttingUnitList");
        }

        public Task ShowFeedback()
        {
            return NavigationService.NavigateAsync("FeedBack");
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            return NavigationService.NavigateAsync($"FixCNT",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}&{NavParams.STRATUM}={stratumCode}"));
        }

        public Task ShowImport()
        {
            return NavigationService.NavigateAsync("Import");
        }

        public async Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            try
            {
                var navResult = await NavigationService.NavigateAsync("LimitingDistance",
                    new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));

                if (navResult != null)
                {
                    Debug.WriteLine(navResult.Success);
                    if (navResult.Exception != null)
                    {
                        Log.LogException("limiting_distance", "", navResult.Exception);
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogException("Navigation", $"Navigating to LimitingDistance", e);
            }
        }

        public Task ShowLogEdit(string logID)
        {
            return NavigationService.NavigateAsync("LogEdit",
                new NavigationParameters($"{NavParams.LogID}={logID}"));
        }

        public async Task ShowLogsList(string treeID)
        {
            try
            {
                var result = await NavigationService.NavigateAsync("LogList",
                new NavigationParameters($"{NavParams.TreeID}={treeID}"));

                var ex = result?.Exception;
                if (ex != null)
                {
                    Log.LogException("navigation", "navigating to treeEdit", ex);
                }
            }
            catch (Exception ex)
            {
                Log.LogException("navigation", "navigating to treeEdit", ex);
            }
        }

        public Task ShowPlotEdit(string plotID)
        {
            return NavigationService.NavigateAsync("PlotEdit",
                new NavigationParameters($"{NavParams.PlotID}={plotID}"));
        }

        public Task ShowPlotEdit(string unitCode, int plotNumber)
        {
            return NavigationService.NavigateAsync("PlotEdit",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        }

        public Task ShowPlotList(string unitCode)
        {
            return NavigationService.NavigateAsync("/Main/Navigation/PlotList",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }

        public Task ShowPlotTally(string plotID)
        {
            return NavigationService.NavigateAsync("PlotTally",
                new NavigationParameters($"{NavParams.PlotID}={plotID}"));
        }

        public Task ShowPlotTally(string unitCode, int plotNumber)
        {
            return NavigationService.NavigateAsync("PlotTally",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        }

        public Task ShowSale(string cruiseID)
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Sale",
                new NavigationParameters($"{NavParams.CruiseID}={cruiseID}"));
        }

        public Task ShowSaleSelect()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/SaleSelect");
        }

        public Task ShowSampleStateManagment()
        {
            return NavigationService.NavigateAsync("SampleStateManagment");
        }

        public Task ShowSettings()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Settings");
        }

        public Task ShowTally(string unitCode)
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Tally",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            return NavigationService.NavigateAsync("ThreePPNTPlot",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            return NavigationService.NavigateAsync("TreeCountEdit",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}&{NavParams.SPECIES}={species}&{NavParams.LIVE_DEAD}={liveDead}"));
        }

        public async Task ShowTreeEdit(string treeID)
        {
            try
            {
                var result = await NavigationService.NavigateAsync("Tree",
                new NavigationParameters($"{NavParams.TreeID}={treeID}"));

                var ex = result.Exception;
                if (ex != null)
                {
                    Log.LogException("navigation", "navigating to treeEdit", ex);
                }
            }
            catch (Exception ex)
            {
                Log.LogException("navigation", "navigating to treeEdit", ex);
            }
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            return NavigationService.NavigateAsync("TreeErrorEdit",
                new NavigationParameters($"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}"));
        }

        public Task ShowTreeList(string unitCode)
        {
            return NavigationService.NavigateAsync("/Main/Navigation/TreeList",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }
    }
}
