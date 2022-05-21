using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using Prism.Commands;
using System;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TreeCountEditViewModel : ViewModelBase
    {
        private string _unitCode;
        private int _treeCountDelta;
        private int _kPIDelta;
        private string _editReason;
        private string _remarks;
        private TallyPopulationEx _tallyPopulation;
        private ICommand _saveTreeCountEditCommand;
        private string _cruiseMethod;
        private ICommand _candelCommand;
        private string _initials;

        public TreeCountEditViewModel(//ICruiseNavigationService navigationService,
            ITallyLedgerDataservice tallyLedgerDataservice,
            ITallyPopulationDataservice tallyPopulationDataservice,
            INatCruiseDialogService dialogService)
        {
            TallyDataservice = tallyLedgerDataservice ?? throw new ArgumentNullException(nameof(tallyLedgerDataservice));
            TallyPopulationDataservice = tallyPopulationDataservice ?? throw new ArgumentNullException(nameof(tallyPopulationDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            //NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public ITallyLedgerDataservice TallyDataservice { get; }

        //public ICruiseNavigationService NavigationService { get; }
        public INatCruiseDialogService DialogService { get; }

        public ICommand SaveTreeCountEditCommand => _saveTreeCountEditCommand ??= new DelegateCommand(SaveEdit);

        public ICommand CancelCommand => _candelCommand ??= new DelegateCommand(ResetInputs);

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

        public string[] EditReasonOptions => new string[] { "Leftover Trees", "Click Counts", "Paper Recorded Counts", "Other" };

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

        //protected override void Load(IParameters parameters)
        //{
        //    if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        //    var unit = parameters.GetValue<string>(NavParams.UNIT);
        //    var stratum = parameters.GetValue<string>(NavParams.STRATUM);
        //    var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
        //    var species = parameters.GetValue<string>(NavParams.SPECIES);
        //    var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);

        //    var tallyPopulation = TallyPopulationDataservice.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

        //    TallyPopulation = tallyPopulation;
        //}

        protected void ResetInputs()
        {
            TreeCountDelta = 0;
            KPIDelta = 0;
            EditReason = null;
            Remarks = null;
            Initials = null;
        }

        public void SaveEdit()
        {
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

            if(TreeCountDelta != 0)
            {
                TallyPopulation.TreeCount += TreeCountDelta;
            }
            if(KPIDelta != 0)
            {
                TallyPopulation.SumKPI += KPIDelta;
            }

            ResetInputs();

            //NavigationService.GoBackAsync();
        }
    }
}