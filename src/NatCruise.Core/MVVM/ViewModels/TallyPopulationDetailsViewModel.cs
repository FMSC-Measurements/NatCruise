using NatCruise.Data;
using NatCruise.Navigation;
using Prism.Common;
using System;

using Stratum = NatCruise.Models.Stratum;
using SampleGroup = NatCruise.Models.SampleGroup;
using NatCruise.Util;

namespace NatCruise.MVVM.ViewModels
{
    public class TallyPopulationDetailsViewModel : ViewModelBase
    {
        private Stratum _stratum;
        private SampleGroup _sampleGroup;

        public TallyPopulationDetailsViewModel(TreeCountEditViewModel treeCountEditViewModel,
            IStratumDataservice stratumDataservice,
            ISampleGroupDataservice sampleGroupDataservice)
        {
            TreeCountEditViewModel = treeCountEditViewModel ?? throw new ArgumentNullException(nameof(treeCountEditViewModel));
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        }

        public TreeCountEditViewModel TreeCountEditViewModel { get; }
        public IStratumDataservice StratumDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }

        public Stratum Stratum
        {
            get => _stratum;
            set => SetProperty(ref _stratum, value);
        }

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            set => SetProperty(ref _sampleGroup, value);
        }

        public override void Initialize(IParameters parameters)
        {
            base.Initialize(parameters);

            var unit = parameters.GetValue<string>(NavParams.UNIT);
            int? plotNumber = parameters.ContainsKey(NavParams.PLOT_NUMBER) ?  parameters.GetValueOrDefault<int>(NavParams.PLOT_NUMBER) : null;
            var stratum = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValue<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);

            Load(unit, stratum, sampleGroup, species, liveDead, plotNumber);
        }

        public void Load(string unit, string stratum, string sampleGroup, string species, string liveDead, int? plotNumber = null)
        {
            Stratum = StratumDataservice.GetStratum(stratum);
            SampleGroup = SampleGroupDataservice.GetSampleGroup(stratum, sampleGroup);

            TreeCountEditViewModel.Load(unit, stratum, sampleGroup, species, liveDead, plotNumber);
        }
    }
}
