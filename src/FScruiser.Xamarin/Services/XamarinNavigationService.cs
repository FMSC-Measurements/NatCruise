using FScruiser.XF.Constants;
using FScruiser.XF.Util;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public class XamarinNavigationService : ICruiseNavigationService
    {
        public XamarinNavigationService(
            INavigationService navigationService,
            ILoggingService loggingService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            Log = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        private ILoggingService Log { get; }
        private INavigationService NavigationService { get; }

        public Task GoBackAsync()
        {
            return NavigationService.GoBackAsync();
        }

        public Task<INavigationResult> NavigateAsync(string path, NavigationParameters navparams)
        {
            return NavigationService.NavigateAsync(path, navparams);
        }

        public Task ShowBlank()
        {
            return NavigationService.NavigateAsync("Blank");
        }

        public Task ShowCruiseLandingLayout()
        {
            return NavigationService.NavigateAsync("/Main/Navigation/Blank", parameters: null, useModalNavigation: false, animated: false);
        }

        public Task ShowCruiseSelect(string saleNumber)
        {
            if (string.IsNullOrEmpty(saleNumber))
            {
                throw new ArgumentException($"'{nameof(saleNumber)}' cannot be null or empty.", nameof(saleNumber));
            }

            return NavigationService.NavigateAsync("CruiseSelect",
                new NavigationParameters($"{NavParams.SaleNumber}={saleNumber}"));
        }

        public Task ShowStrata()
        {
            return NavigationService.NavigateAsync("Navigation/Strata");
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

        public Task ShowStratumDetail(string stratumCode)
        {
            return NavigationService.NavigateAsync("StratumDetail",
                new NavigationParameters($"{NavParams.STRATUM}={stratumCode}"));
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            return NavigationService.NavigateAsync("FieldSetup",
                new NavigationParameters($"{NavParams.STRATUM}={stratumCode}"));
        }

        public Task ShowFeedback()
        {
            return NavigationService.NavigateAsync("FeedBack");
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            throw new NotImplementedException();
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

        public Task ShowManageCruisers()
        {
            return NavigationService.NavigateAsync("Navigation/Cruisers");
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

        public Task ShowPlotTreeList(string unitCode)
        {
            if (unitCode is null) { throw new ArgumentNullException(nameof(unitCode)); }

            return NavigationService.NavigateAsync("Navigation/PlotTreeList",
                new NavigationParameters($"{NavParams.UNIT}={unitCode}"));
        }

        //public Task ShowPlotTally(string unitCode, int plotNumber)
        //{
        //    return NavigationService.NavigateAsync("PlotTally",
        //        new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.PLOT_NUMBER}={plotNumber}"));
        //}

        public Task ShowPrivacyPolicy()
        {
            return NavigationService.NavigateAsync("PrivacyPolicy");
        }

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

        public Task ShowDatabaseUtilities()
        {
            return NavigationService.NavigateAsync("DatabaseUtilities", parameters: null, useModalNavigation: true, animated: false);
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

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            if (unitCode is null) { throw new ArgumentNullException(nameof(unitCode)); }

            if (plotNumber < 1) { throw new ArgumentOutOfRangeException(nameof(plotNumber), plotNumber, ""); }

            if (string.IsNullOrEmpty(stratumCode)) { throw new ArgumentException($"'{nameof(stratumCode)}' cannot be null or empty.", nameof(stratumCode)); }

            if (string.IsNullOrEmpty(sampleGroupCode)) { throw new ArgumentException($"'{nameof(sampleGroupCode)}' cannot be null or empty.", nameof(sampleGroupCode)); }

            var parameters = new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.PLOT_NUMBER}={plotNumber}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}");

            return NavigationService.NavigateAsync("PlotTallyPopulationDetails",
                parameters);
        }

        public Task ShowTreeAuditRules()
        {
            return NavigationService.NavigateAsync("Navigation/TreeAuditRuleList");
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            return NavigationService.NavigateAsync("TreeAuditRuleEdit",
                new NavigationParameters($"{NavParams.TreeAuditRuleID}={tarID}"));
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

            var parameters = new NavigationParameters($"{NavParams.UNIT}={unitCode}&{NavParams.STRATUM}={stratumCode}" +
                $"&{NavParams.SAMPLE_GROUP}={sampleGroupCode}" +
                $"&{NavParams.SPECIES}={species}" +
                $"&{NavParams.LIVE_DEAD}={liveDead}");

            return NavigationService.NavigateAsync("TreeCountEdit",
                parameters);
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

        public Task ShowUserAgreement()
        {
            return NavigationService.NavigateAsync("UserAgreement");
        }

        public Task ShowUtilities()
        {
            return NavigationService.NavigateAsync("Navigation/Utilities");
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            return NavigationService.NavigateAsync("SampleGroups",
                new NavigationParameters($"{NavParams.STRATUM}={stratumCode}"));
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            return NavigationService.NavigateAsync("Subpopulations",
                new NavigationParameters($"{NavParams.STRATUM}={stratumCode}&{NavParams.SAMPLE_GROUP}={sampleGroupCode}"));
        }
    }
}