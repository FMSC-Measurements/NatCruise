using Microsoft.Extensions.DependencyInjection;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise
{
    public static class IServiceCollectionExtentions
    {
        private static readonly ServiceDescriptor[] SERVICE_DEFINITIONS = new[] {


                ServiceDescriptor.Transient<ICruiseLogDataservice, CruiseLogDataservice>(),
                ServiceDescriptor.Transient<ICuttingUnitDataservice, CuttingUnitDataservice>(),
                ServiceDescriptor.Transient<IFieldSetupDataservice, FieldSetupDataservice>(),
                ServiceDescriptor.Transient<IFixCNTDataservice, FixCNTDataservice>(),
                ServiceDescriptor.Transient<ILogDataservice, LogDataservice>(),
                ServiceDescriptor.Transient<ILogErrorDataservice, LogErrorDataservice>(),
                ServiceDescriptor.Transient<ILogFieldDataservice, LogFieldDataservice>(),
                ServiceDescriptor.Transient<IMessageLogDataservice, MessageLogDataservice>(),

                ServiceDescriptor.Transient<IPlotDataservice, PlotDataservice>(),
                ServiceDescriptor.Transient<IPlotErrorDataservice, PlotErrorDataservice>(),
                ServiceDescriptor.Transient<IPlotStratumDataservice, PlotStratumDataservice>(),
                ServiceDescriptor.Transient<IPlotTreeDataservice, PlotTreeDataservice>(),
                ServiceDescriptor.Transient<ISaleDataservice, SaleDataservice>(),
                ServiceDescriptor.Transient<ISampleGroupDataservice, SampleGroupDataservice>(),
                ServiceDescriptor.Transient<ISamplerStateDataservice, SamplerStateDataservice>(),
                ServiceDescriptor.Transient<ISampleSelectorDataService>(x => x.GetRequiredService<IDataContextService>().SampleSelectorDataService),
                // Setup Info creates its own database and doesn't use the data context
                // because of this we only want a single instance
                ServiceDescriptor.Singleton<ISetupInfoDataservice, SetupInfoDataservice>(),
                ServiceDescriptor.Transient<ISpeciesDataservice, SpeciesDataservice>(),
                ServiceDescriptor.Transient<IStratumDataservice, StratumDataservice>(),
                ServiceDescriptor.Transient<IStratumTemplateDataservice, StratumTemplateDataservice>(),
                ServiceDescriptor.Transient<ISubpopulationDataservice, SubpopulationDataservice>(),
                ServiceDescriptor.Transient<ITallyDataservice, TallyDataservice>(),
                ServiceDescriptor.Transient<ITallyLedgerDataservice, TallyLedgerDataservice>(),
                ServiceDescriptor.Transient<ITallyPopulationDataservice, TallyPopulationDataservice>(),
                ServiceDescriptor.Transient<ITreeDataservice, TreeDataservice>(),
                ServiceDescriptor.Transient<ITreeAuditRuleDataservice, TreeAuditRuleDataservice>(),
                ServiceDescriptor.Transient<ITreeDefaultValueDataservice, TreeDefaultValueDataservice>(),
                ServiceDescriptor.Transient<ITreeErrorDataservice, TreeErrorDataservice>(),
                ServiceDescriptor.Transient<ITreeFieldDataservice, TreeFieldDataservice>(),
                ServiceDescriptor.Transient<ITreeFieldValueDataservice, TreeFieldValueDataservice>(),
            };


        public static IServiceCollection AddNatCruiseCoreDataservices(this IServiceCollection @this)
        {
            foreach (var serviceDef in SERVICE_DEFINITIONS)
            {
                @this.Add(serviceDef);
            }

            return @this;
        }
    }
}
