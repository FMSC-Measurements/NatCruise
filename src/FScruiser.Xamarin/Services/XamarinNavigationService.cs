﻿using FScruiser.XF.Constants;
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

        public Task ShowBlank()
        {
            return NavigationService.NavigateAsync("Blank");
        }

        public Task<INavigationResult> NavigateAsync(string path, NavigationParameters navparams)
        {
            return NavigationService.NavigateAsync(path, navparams);
        }

        public Task ShowManageCruisers()
        {
            return NavigationService.NavigateAsync("Navigation/Cruisers");
        }

        public Task ShowCruiseLandingLayout()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Blank", parameters: null, useModalNavigation: false, animated: false);
        }

        public Task ShowCruiseSelect(string saleID)
        {
            if (string.IsNullOrEmpty(saleID))
            {
                throw new ArgumentException($"'{nameof(saleID)}' cannot be null or empty.", nameof(saleID));
            }

            return NavigationService.NavigateAsync("CruiseSelect",
                new NavigationParameters($"{NavParams.SaleID}={saleID}"));
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            return NavigationService.NavigateAsync("Navigation/CuttingUnitInfo",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }


        public Task ShowCuttingUnitList()
        {
            return NavigationService.NavigateAsync("Navigation/CuttingUnitList");
        }

        public Task ShowFeedback()
        {
            return NavigationService.NavigateAsync("FeedBack");
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            if (string.IsNullOrEmpty(stratumCode))
            {
                throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty.", nameof(stratumCode));
            }

            return NavigationService.NavigateAsync($"FixCNTTally",
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
            if (string.IsNullOrEmpty(logID))
            {
                throw new ArgumentException($"'{nameof(logID)}' cannot be null or empty.", nameof(logID));
            }

            return NavigationService.NavigateAsync("LogEdit",
                new NavigationParameters($"{NavParams.LogID}={logID}"));
        }

        public async Task ShowLogsList(string treeID)
        {
            if (string.IsNullOrEmpty(treeID))
            {
                throw new ArgumentException($"'{nameof(treeID)}' cannot be null or empty.", nameof(treeID));
            }

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
            if (string.IsNullOrEmpty(plotID))
            {
                throw new ArgumentException($"'{nameof(plotID)}' cannot be null or empty.", nameof(plotID));
            }

            return NavigationService.NavigateAsync("PlotEdit",
                new NavigationParameters($"{NavParams.PlotID}={plotID}"));
        }

        //public Task ShowPlotEdit(string unitCode, int plotNumber)
        //{
        //    return NavigationService.NavigateAsync("PlotEdit",
        //        new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        //}

        public Task ShowPlotList(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            return NavigationService.NavigateAsync("Navigation/PlotList",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }

        public Task ShowPlotTally(string plotID)
        {
            if (string.IsNullOrEmpty(plotID))
            {
                throw new ArgumentException($"'{nameof(plotID)}' cannot be null or empty.", nameof(plotID));
            }

            return NavigationService.NavigateAsync("PlotTally",
                new NavigationParameters($"{NavParams.PlotID}={plotID}"));
        }

        //public Task ShowPlotTally(string unitCode, int plotNumber)
        //{
        //    return NavigationService.NavigateAsync("PlotTally",
        //        new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        //}

        public Task ShowSale(string cruiseID)
        {
            if (string.IsNullOrEmpty(cruiseID))
            {
                throw new ArgumentException($"'{nameof(cruiseID)}' cannot be null or empty.", nameof(cruiseID));
            }

            return NavigationService.NavigateAsync("Navigation/Sale",
                new NavigationParameters($"{NavParams.CruiseID}={cruiseID}"));
        }

        public Task ShowSaleSelect()
        {
            return NavigationService.NavigateAsync("Navigation/SaleSelect");
        }

        public Task ShowSampleStateManagment()
        {
            return NavigationService.NavigateAsync("SampleStateManagment");
        }

        public Task ShowSettings()
        {
            return NavigationService.NavigateAsync("Navigation/Settings");
        }

        public Task ShowTally(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            return NavigationService.NavigateAsync("Navigation/Tally",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            if (string.IsNullOrEmpty(stratumCode))
            {
                throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty.", nameof(stratumCode));
            }

            return NavigationService.NavigateAsync("ThreePPNTPlot",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            if (string.IsNullOrEmpty(stratumCode))
            {
                throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty.", nameof(stratumCode));
            }

            if (string.IsNullOrEmpty(sampleGroupCode))
            {
                throw new ArgumentException($"'{nameof(sampleGroupCode)}' cannot be null or empty.", nameof(sampleGroupCode));
            }

            return NavigationService.NavigateAsync("TreeCountEdit",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}&{NavParams.SPECIES}={species}&{NavParams.LIVE_DEAD}={liveDead}"));
        }

        public async Task ShowTreeEdit(string treeID)
        {
            if (string.IsNullOrEmpty(treeID))
            {
                throw new ArgumentException($"'{nameof(treeID)}' cannot be null or empty.", nameof(treeID));
            }

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
            if (string.IsNullOrEmpty(treeID))
            {
                throw new ArgumentException($"'{nameof(treeID)}' cannot be null or empty.", nameof(treeID));
            }

            if (string.IsNullOrEmpty(treeAuditRuleID))
            {
                throw new ArgumentException($"'{nameof(treeAuditRuleID)}' cannot be null or empty.", nameof(treeAuditRuleID));
            }

            return NavigationService.NavigateAsync("TreeErrorEdit",
                new NavigationParameters($"{NavParams.TreeID}={treeID}&{NavParams.TreeAuditRuleID}={treeAuditRuleID}"));
        }

        public Task ShowTreeList(string unitCode)
        {
            if (string.IsNullOrEmpty(unitCode))
            {
                throw new ArgumentException($"'{nameof(unitCode)}' cannot be null or empty.", nameof(unitCode));
            }

            return NavigationService.NavigateAsync("Navigation/TreeList",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }
    }
}
