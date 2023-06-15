using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM.ViewModels
{
    public class PlotTallyPopulationDetailsViewModel : ViewModelBase
    {
        private TallyPopulation _tallyPopulation;
        private SampleGroup _sampleGroup;

        public PlotTallyPopulationDetailsViewModel(ITallyPopulationDataservice tallyPopulationDataservice, ISampleGroupDataservice sampleGroupDataservice)
        {
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException( nameof(sampleGroupDataservice));
        }

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }

        public TallyPopulation TallyPopulation
        {
            get => _tallyPopulation;
            set
            {
                SetProperty(ref _tallyPopulation, value);
            }
        }

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            private set => SetProperty(ref _sampleGroup, value);
        }

        public override void Initialize(IParameters parameters)
        {
            base.Initialize(parameters);

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroupCode = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValueOrDefault<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValueOrDefault<string>(NavParams.LIVE_DEAD);

            TallyPopulation = TallyPopulationDataservice.GetPlotTallyPopulation(unitCode, plotNumber, stratumCode, sampleGroupCode, species, liveDead);
            SampleGroup = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);
        }
    }
}
