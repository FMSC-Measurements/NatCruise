using CommunityToolkit.Mvvm.ComponentModel;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public partial class TallyLedgerListViewModel : ViewModelBase
    {
        [ObservableProperty]
        private IEnumerable<TallyLedger> _tallyLedgers;

        [ObservableProperty]
        private string _cuttingUnitCode;

        [ObservableProperty]
        private string _stratumCode;

        [ObservableProperty]
        private string _sampleGroupCode;

        [ObservableProperty]
        private string _speciesCode;

        [ObservableProperty]
        private string _liveDead;

        public TallyLedgerListViewModel(ITallyLedgerDataservice tallyLedgerDataservice)
        {
            TallyLedgerDataservice = tallyLedgerDataservice ?? throw new ArgumentNullException(nameof(tallyLedgerDataservice));
        }

        public ITallyLedgerDataservice TallyLedgerDataservice { get; }

        public void Load(string cuttingUnitCode, string stratumCode, string sampleGroupCode, string speciesCode, string liveDead)
        {
            CuttingUnitCode = cuttingUnitCode;
            StratumCode = stratumCode;
            SampleGroupCode = sampleGroupCode;
            SpeciesCode = speciesCode;
            LiveDead = liveDead;

            TallyLedgers = TallyLedgerDataservice.GetTallyLedgers(cuttingUnitCode, stratumCode, sampleGroupCode, speciesCode, liveDead);
        }
    }
}