using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TallyLedgerListViewModel : ViewModelBase
    {
        private IEnumerable<TallyLedger> _tallyLedgers;
        private string _speciesCode;
        private string _liveDead;
        private string _sampleGroupCode;
        private string _stratumCode;
        private string _cuttingUnitCode;

        public TallyLedgerListViewModel(ITallyLedgerDataservice tallyLedgerDataservice)
        {
            TallyLedgerDataservice = tallyLedgerDataservice ?? throw new ArgumentNullException(nameof(tallyLedgerDataservice));
        }

        public ITallyLedgerDataservice TallyLedgerDataservice { get; }

        private bool _isLoading;

        public IEnumerable<TallyLedger> TallyLedgers
        {
            get => _tallyLedgers;
            set => SetProperty(ref _tallyLedgers, value);
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

        public string SpeciesCode
        {
            get => _speciesCode;
            set
            {
                SetProperty(ref _speciesCode, value);
                Load();
            }
        }

        public string LiveDead
        {
            get => _liveDead;
            set
            {
                SetProperty(ref _liveDead, value);
                Load();
            }
        }

        public override void Load()
        {
            base.Load();

            Load(CuttingUnitCode, StratumCode, SampleGroupCode, SpeciesCode, LiveDead);
        }

        public void Load(string cuttingUnitCode, string stratumCode, string sampleGroupCode, string speciesCode, string liveDead)
        {
            _isLoading = true;

            TallyLedgers = TallyLedgerDataservice.GetTallyLedgers(cuttingUnitCode, stratumCode, sampleGroupCode, SpeciesCode, LiveDead);
            
        }
    }
}