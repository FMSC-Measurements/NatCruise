using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.MVVM.ViewModels
{
    public partial class TreeCountEditViewModel : ViewModelBase
    {
        public const string EDITREASON_LEFTOVERTREES = "Leftover Trees";
        public const string EDITREASON_CLICKERCOUNTS = "Clicker Counts";
        public const string EDITREASON_PAPERCOUNTS = "Paper Recorded Counts";
        public const string EDITREASON_OTHER = "Other";

        private int _treeCountDelta;
        private int _kPIDelta;
        private string _editReason;
        private string _remarks;
        private TallyPopulation _tallyPopulation;
        private string _cruiseMethod;
        private string _initials;
        private IEnumerable<string> _cruisers;

        public TreeCountEditViewModel(INatCruiseNavigationService navigationService,
            ITallyLedgerDataservice tallyLedgerDataservice,
            ITallyPopulationDataservice tallyPopulationDataservice,
            ICruisersDataservice cruisersDataservice,
            INatCruiseDialogService dialogService)
        {
            TallyDataservice = tallyLedgerDataservice ?? throw new ArgumentNullException(nameof(tallyLedgerDataservice));
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            CruisersDataservice = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        #region Events

        public event EventHandler TreeCountModified;

        #endregion Events

        #region Services

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public ICruisersDataservice CruisersDataservice { get; }
        public ITallyLedgerDataservice TallyDataservice { get; }
        public INatCruiseNavigationService NavigationService { get; }
        public INatCruiseDialogService DialogService { get; }

        #endregion Services

        #region Properties

        // Generated Properties
        [ObservableProperty]
        private bool _isPlot;

        [ObservableProperty]
        private bool _canEditTreeCount;

        [ObservableProperty]
        private bool _isSTR;

        [ObservableProperty]
        private bool _is3P;

        [ObservableProperty]
        private string _unitCode;

        public TallyPopulation TallyPopulation
        {
            get => _tallyPopulation;

            protected set
            {
                SetProperty(ref _tallyPopulation, value);
                OnTallyPopulationChanged(value);
            }
        }

        private void OnTallyPopulationChanged(TallyPopulation tallyPopulation)
        {
            if (tallyPopulation != null)
            {
                UnitCode = tallyPopulation.CuttingUnitCode;
                CruiseMethod = tallyPopulation.Method;
                Cruisers = CruisersDataservice.GetCruisers();
            }
            else
            {
                UnitCode = null;
                CruiseMethod = null;
            }
        }

        public string CruiseMethod
        {
            get => _cruiseMethod;
            set
            {
                SetProperty(ref _cruiseMethod, value);
                if (value != null)
                {
                    IsPlot = CruiseMethods.PLOT_METHODS.Contains(value);
                    CanEditTreeCount = !IsPlot;
                    Is3P = CruiseMethods.THREE_P_METHODS.Contains(value);
                    IsSTR = CruiseMethods.STR == value;
                }

                OnPropertyChanged(nameof(IsSTR));
                OnPropertyChanged(nameof(Is3P));
                OnPropertyChanged(nameof(CanEditTreeCount));
            }
        }

        public int TreeCountDelta
        {
            get { return _treeCountDelta; }
            set
            {
                SetProperty(ref _treeCountDelta, value);
            }
        }

        public int KPIDelta
        {
            get => _kPIDelta;
            set
            {
                SetProperty(ref _kPIDelta, value);
            }
        }

        public string Initials
        {
            get => _initials;
            set => SetProperty(ref _initials, value);
        }

        public string[] EditReasonOptions => new string[] { EDITREASON_LEFTOVERTREES, EDITREASON_CLICKERCOUNTS, EDITREASON_PAPERCOUNTS, EDITREASON_OTHER };

        public string EditReason
        {
            get => _editReason;
            set => SetProperty(ref _editReason, value);
        }

        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public IEnumerable<string> Cruisers
        {
            get => _cruisers;
            set => SetProperty(ref _cruisers, value);
        }

        #endregion Properties

        protected override void OnInitialize(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var stratum = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValue<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);

            Load(unit, stratum, sampleGroup, species, liveDead);
        }

        public void Load(string unit, string stratum, string sampleGroup, string species, string liveDead, int? plotNumber = null)
        {
            if (plotNumber == null)
            {
                TallyPopulation = TallyPopulationDataservice.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);
            }
            else
            {
                TallyPopulation = TallyPopulationDataservice.GetPlotTallyPopulation(unit, plotNumber.Value, stratum, sampleGroup, species, liveDead);
                IsPlot = true;
            }
        }

        [RelayCommand]
        protected void ResetInputs()
        {
            TreeCountDelta = 0;
            KPIDelta = 0;
            EditReason = null;
            Remarks = null;
            Initials = null;
        }

        [RelayCommand]
        public void AddTallyLedgerEntry()
        {
            if (TreeCountDelta == 0 && KPIDelta == 0) { return; }
            var tp = TallyPopulation;
            //var cruiser = DialogService.AskCruiserAsync();
            //if (cruiser == null) { return; }

            var tallyLedger = new TallyLedger(tp.CuttingUnitCode, tp);
            tallyLedger.TreeCount = TreeCountDelta;
            tallyLedger.KPI = KPIDelta;
            tallyLedger.Reason = EditReason;
            tallyLedger.Remarks = Remarks;
            tallyLedger.Signature = Initials;
            tallyLedger.EntryType = TallyLedgerEntryTypeValues.TREECOUNT_EDIT;

            TallyDataservice.InsertTallyLedger(tallyLedger);

            if (TreeCountDelta != 0)
            {
                tp.TreeCount += TreeCountDelta;
                tp.TreeCountCruise += TreeCountDelta;
                if (IsPlot)
                { tp.TreeCountPlot += TreeCountDelta; }
            }
            if (KPIDelta != 0)
            {
                tp.SumKPI += KPIDelta;
                tp.SumKPICruise += KPIDelta;
                if (IsPlot)
                { tp.SumKPIPlot += KPIDelta; }
            }

            ResetInputs();
            TreeCountModified?.Invoke(this, EventArgs.Empty);

            //return NavigationService.GoBackAsync();
        }
    }
}