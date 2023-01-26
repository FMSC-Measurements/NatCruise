using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using NatCruise.Util;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class TreeCountEditViewModel : ViewModelBase
    {
        public const string EDITREASON_LEFTOVERTREES = "Leftover Trees";
        public const string EDITREASON_CLICKERCOUNTS = "Clicker Counts";
        public const string EDITREASON_PAPERCOUNTS = "Paper Recorded Counts";
        public const string EDITREASON_OTHER = "Other";

        private string _unitCode;
        private int _treeCountDelta;
        private int _kPIDelta;
        private string _editReason;
        private string _remarks;
        private TallyPopulationEx _tallyPopulation;
        private ICommand _saveTreeCountEditCommand;
        private string _cruiseMethod;
        private ICommand _cancelCommand;
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
        #endregion

        #region Services
        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public ICruisersDataservice CruisersDataservice { get; }
        public ITallyLedgerDataservice TallyDataservice { get; }
        public INatCruiseNavigationService NavigationService { get; }
        public INatCruiseDialogService DialogService { get; }
        #endregion

        #region Commands
        public ICommand SaveTreeCountEditCommand => _saveTreeCountEditCommand ??= new DelegateCommand(() => SaveEdit().FireAndForget());

        public ICommand CancelCommand => _cancelCommand ??= new DelegateCommand(() => NavigationService.GoBackAsync().FireAndForget());
        #endregion

        #region Properties
        public string UnitCode
        {
            get { return _unitCode; }
            set { SetProperty(ref _unitCode, value); }
        }

        public TallyPopulationEx TallyPopulation
        {
            get => _tallyPopulation;

            set
            {
                SetProperty(ref _tallyPopulation, value);
                OnTallyPopulationChanged(value);
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(TallyPopulationDescription));
                RaisePropertyChanged(nameof(TreeCount));
            }
        }

        private void OnTallyPopulationChanged(TallyPopulationEx tallyPopulation)
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

        public string StratumCode => TallyPopulation?.StratumCode;

        public string TallyPopulationDescription => TallyPopulation?.TallyDescription;

        public int? TreeCount => TallyPopulation?.TreeCount;

        public int TreeCountDelta
        {
            get { return _treeCountDelta; }
            set
            {
                SetProperty(ref _treeCountDelta, value);
                RaisePropertyChanged(nameof(AdjustedTreeCount));
            }
        }

        public int KPIDelta
        {
            get => _kPIDelta;
            set
            {
                SetProperty(ref _kPIDelta, value);
                RaisePropertyChanged(nameof(AdjustedSumKPI));
            }
        }

        public int AdjustedSumKPI
        {
            get => (TallyPopulation?.SumKPI ?? 0) + KPIDelta;
        }

        public string Initials
        {
            get => _initials;
            set => SetProperty(ref _initials, value);
        }

        public string CruiseMethod
        {
            get => _cruiseMethod;
            set
            {
                SetProperty(ref _cruiseMethod, value);
                RaisePropertyChanged(nameof(IsSTR));
                RaisePropertyChanged(nameof(Is3P));
            }
        }

        public bool IsSTR => CruiseMethod == CruiseDAL.Schema.CruiseMethods.STR;

        public bool Is3P => CruiseDAL.Schema.CruiseMethods.THREE_P_METHODS.Contains(CruiseMethod);

        public int AdjustedTreeCount
        {
            get => (TreeCount ?? 0) + TreeCountDelta;
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

        #endregion

        protected override void OnInitialize(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            

            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var stratum = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValue<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);
            //var isClickerTally = parameters.GetValueOrDefault<bool>(NavParams.IS_CLICKER_TALLY, false);

            var tallyPopulation = TallyPopulationDataservice.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

            TallyPopulation = tallyPopulation;

            //if (isClickerTally)
            //{
            //    EditReason = CLICKERCOUNTS;
            //    TreeCountDelta = tallyPopulation.Frequency;
            //}
        }

        protected void ResetInputs()
        {
            TreeCountDelta = 0;
            KPIDelta = 0;
            EditReason = null;
            Remarks = null;
            Initials = null;
        }

        public Task SaveEdit()
        {
            if (TreeCountDelta == 0 && KPIDelta == 0) { return Task.CompletedTask; }

            //var cruiser = DialogService.AskCruiserAsync();
            //if (cruiser == null) { return; }

            var tallyLedger = new TallyLedger(UnitCode, TallyPopulation);
            tallyLedger.TreeCount = TreeCountDelta;
            tallyLedger.KPI = KPIDelta;
            tallyLedger.Reason = EditReason;
            tallyLedger.Remarks = Remarks;
            tallyLedger.Signature = Initials;
            tallyLedger.EntryType = TallyLedgerEntryTypeValues.TREECOUNT_EDIT;

            TallyDataservice.InsertTallyLedger(tallyLedger);

            if (TreeCountDelta != 0)
            {
                TallyPopulation.TreeCount += TreeCountDelta;
            }
            if (KPIDelta != 0)
            {
                TallyPopulation.SumKPI += KPIDelta;
            }

            ResetInputs();
            TreeCountModified?.Invoke(this, EventArgs.Empty);

            return NavigationService.GoBackAsync();
        }
    }
}
