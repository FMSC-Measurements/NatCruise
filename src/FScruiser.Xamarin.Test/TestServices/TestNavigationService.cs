using FScruiser.XF.Services;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Threading.Tasks;

namespace FScruiser.XF
{
    public class TestNavigationService : ICruiseNavigationService
    {
        //public INavigationService NavigationService { get; }

        public TestNavigationService()//INavigationService navigationService)
        {
            //NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public Task ShowImport()
        {
            throw new NotImplementedException();
        }

        public Task ShowCruiseLandingLayout()
        {
            throw new NotImplementedException();
        }

        public Task ShowCruiseSelect(string saleID)
        {
            throw new NotImplementedException();
        }

        public Task ShowCuttingUnitList()
        {
            throw new NotImplementedException();
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowLimitingDistance(string unitCode, string stratumCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowLogEdit(string logID)
        {
            throw new NotImplementedException();
        }

        public Task ShowLogsList(string treeID)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotEdit(string plotID)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotEdit(string unitCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotList(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotTally(string plotID)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotTally(string unitCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowSaleSelect()
        {
            throw new NotImplementedException();
        }

        public Task ShowSale()
        {
            throw new NotImplementedException();
        }

        public Task ShowTally(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeCountEdit(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeEdit(string treeID)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeErrorEdit(string treeID, string treeAuditRuleID)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeList(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task GoBackAsync()
        {
            throw new NotImplementedException();
        }

        public Task ShowFeedback()
        {
            throw new NotImplementedException();
        }

        public Task ShowSettings()
        {
            throw new NotImplementedException();
        }

        public Task ShowSampleStateManagment()
        {
            throw new NotImplementedException();
        }

        public Task ShowManageCruisers()
        {
            throw new NotImplementedException();
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowSale(string cruiseID)
        {
            throw new NotImplementedException();
        }

        public Task ShowBlank()
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowUserAgreement()
        {
            throw new NotImplementedException();
        }

        public Task ShowPrivacyPolicy()
        {
            throw new NotImplementedException();
        }

        public Task ShowUtilities()
        {
            throw new NotImplementedException();
        }
    }
}