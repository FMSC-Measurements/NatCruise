using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TallyPopulationListViewModel : ViewModelBase
    {
        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<TallyPopulation> _tallyPopulations;
        private TallyPopulation _selectedTallyPopulation;
        private TreeCountEditViewModel _treeCountEditViewModel;

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

        public IEnumerable<TallyPopulation> TallyPopulations
        {
            get => _tallyPopulations;
            set => SetProperty(ref _tallyPopulations, value);
        }

        public TallyPopulation SelectedTallyPopulation
        {
            get => _selectedTallyPopulation;
            set
            {
                SetProperty(ref _selectedTallyPopulation, value);
                TreeCountEditViewModel.Load(value?.CuttingUnitCode, value?.StratumCode, value?.SampleGroupCode, value?.SpeciesCode, value?.LiveDead);
                TallyLedgerListViewModel.Load(value?.CuttingUnitCode, value?.StratumCode, value?.SampleGroupCode, value?.SpeciesCode, value?.LiveDead);
            }
        }

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public TreeCountEditViewModel TreeCountEditViewModel
        {
            get => _treeCountEditViewModel;
            private set
            {
                if (_treeCountEditViewModel != null) { _treeCountEditViewModel.TreeCountModified -= TreeCountEditViewModel_TreeCountModified; }
                _treeCountEditViewModel = value;
                if(value != null) { _treeCountEditViewModel.TreeCountModified += TreeCountEditViewModel_TreeCountModified;}
            }
        }

        private void TreeCountEditViewModel_TreeCountModified(object sender, EventArgs e)
        {
            TallyLedgerListViewModel.Load();
        }

        public TallyLedgerListViewModel TallyLedgerListViewModel { get; }

        public override void Load()
        {
            base.Load();

            TallyPopulations = TallyPopulationDataservice.GetTallyPopulations(CuttingUnitCode, StratumCode, SampleGroupCode);
        }
    }
}