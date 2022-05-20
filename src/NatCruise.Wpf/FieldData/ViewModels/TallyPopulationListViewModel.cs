using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TallyPopulationListViewModel : ViewModelBase
    {
        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<TallyPopulationEx> _tallyPopulations;
        private TallyPopulationEx _selectedTallyPopulation;
        private string _speciesCode;
        private string _liveDead;

        public TallyPopulationListViewModel(ITallyPopulationDataservice tallyPopulationDataservice, TreeCountEditViewModel treeCountEditViewModel, TallyLedgerListViewModel tallyLedgerListViewModel)
        {
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            TreeCountEditViewModel = treeCountEditViewModel ?? throw new ArgumentNullException(nameof(treeCountEditViewModel));
            TallyLedgerListViewModel = tallyLedgerListViewModel ?? throw new ArgumentNullException(nameof(tallyLedgerListViewModel));
        }

        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set
            {
                SetProperty(ref _cuttingUnitCode, value);
                Load();
            }
        }

        public string StratumCode
        {
            get => _stratumCode;
            set
            {
                SetProperty(ref _stratumCode, value);
                Load();
            }
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set
            {
                SetProperty(ref _sampleGroupCode, value);
                Load();
            }
        }

        public IEnumerable<TallyPopulationEx> TallyPopulations
        {
            get => _tallyPopulations;
            set => SetProperty(ref _tallyPopulations, value);
        }

        public TallyPopulationEx SelectedTallyPopulation
        {
            get => _selectedTallyPopulation;
            set
            {
                SetProperty(ref _selectedTallyPopulation, value);
                TreeCountEditViewModel.TallyPopulation = value;
                TallyLedgerListViewModel.Load(value?.CuttingUnitCode, value?.StratumCode, value?.SampleGroupCode, value?.SpeciesCode, value?.LiveDead);
            }
        }

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public TreeCountEditViewModel TreeCountEditViewModel { get; }
        public TallyLedgerListViewModel TallyLedgerListViewModel { get; }

        public override void Load()
        {
            base.Load();

            TallyPopulations = TallyPopulationDataservice.GetTallyPopulations(CuttingUnitCode, StratumCode, SampleGroupCode);
        }
    }
}