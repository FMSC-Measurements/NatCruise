﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Maui.Services
{
    public class MauiNavigationService : ICruiseNavigationService
    {
        public MauiNavigationService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _shell = new Lazy<Shell>(() => ServiceProvider.GetRequiredService<AppShell>());
        }

        Lazy<Shell> _shell;

        public Shell Shell => _shell.Value;
            
        public IServiceProvider ServiceProvider { get; }

        public Task GoBackAsync()
        {
            throw new NotImplementedException();
        }

        public Task ShowAbout()
        {
            throw new NotImplementedException();
        }

        public Task ShowBlank()
        {
            throw new NotImplementedException();
        }

        public Task ShowCruiseLandingLayout()
        {
            throw new NotImplementedException();
        }

        public Task ShowCruiseSelect(string saleNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowCuttingUnitInfo(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowCuttingUnitList()
        {
            throw new NotImplementedException();
        }

        public Task ShowDatabaseUtilities()
        {
            throw new NotImplementedException();
        }

        public Task ShowFeedback()
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldData(string cuttingUnit = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowFieldSetup(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowFixCNT(string unitCode, int plotNumber, string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowImport()
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

        public Task ShowManageCruisers()
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotEdit(string plotID)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotList(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotTally(string? plotID)
        {
            throw new NotImplementedException();
        }

        public Task ShowPlotTreeList(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowPrivacyPolicy()
        {
            throw new NotImplementedException();
        }

        public Task ShowSale(string cruiseID)
        {
            throw new NotImplementedException();
        }

        public Task ShowSaleSelect()
        {
            throw new NotImplementedException();
        }

        public Task ShowSampleGroups(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowSampleStateManagment()
        {
            throw new NotImplementedException();
        }

        public Task ShowSettings()
        {
            throw new NotImplementedException();
        }

        public Task ShowStrata()
        {
            throw new NotImplementedException();
        }

        public Task ShowStratumInfo(string stratumCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowSubpopulations(string stratumCode, string sampleGroupCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowTally(string unitCode)
        {
            throw new NotImplementedException();
        }

        public Task ShowTallyPopulationInfo(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            throw new NotImplementedException();
        }

        public Task ShowTallyPopulationInfo(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            throw new NotImplementedException();
        }

        public Task ShowThreePPNTPlot(string unitCode, string stratumCode, int plotNumber)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeAuditRuleEdit(string tarID)
        {
            throw new NotImplementedException();
        }

        public Task ShowTreeAuditRules()
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

        public Task ShowUserAgreement()
        {
            throw new NotImplementedException();
        }

        public Task ShowUtilities()
        {
            throw new NotImplementedException();
        }
    }
}
