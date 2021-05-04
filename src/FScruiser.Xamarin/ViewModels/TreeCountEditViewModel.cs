using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using Prism.Common;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeCountEditViewModel : XamarinViewModelBase
    {
        private bool _isSaved;
        private string _unitCode;
        private int _treeCountDelta;
        private int _kPIDelta;
        private string _editReason;
        private string _remarks;
        private TallyPopulation _tallyPopulation;
        private ICommand _saveTreeCountEditCommand;
        private string _cruiseMethod;

        public TreeCountEditViewModel(ICruiseNavigationService navigationService, IDataserviceProvider datastoreProvider, ICruiseDialogService dialogService)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            TallyDataservice = datastoreProvider.GetDataservice<ITallyDataservice>();
            TallyPopulationDataservice = datastoreProvider.GetDataservice<ITallyPopulationDataservice>();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public ITallyPopulationDataservice TallyPopulationDataservice { get; }
        public ITallyDataservice TallyDataservice { get; }
        public ICruiseNavigationService NavigationService { get; }
        public ICruiseDialogService DialogService { get; }

        public ICommand SaveTreeCountEditCommand => _saveTreeCountEditCommand ?? (_saveTreeCountEditCommand = new Command(SaveEdit));

        public string UnitCode
        {
            get { return _unitCode; }
            set { SetProperty(ref _unitCode, value); }
        }

        public TallyPopulation TallyPopulation
        {
            get => _tallyPopulation;

            set
            {
                SetProperty(ref _tallyPopulation, value);
                RaisePropertyChanged(nameof(StratumCode));
                RaisePropertyChanged(nameof(TallyPopulationDescription));
                RaisePropertyChanged(nameof(TreeCount));
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

        

        public int KPIDelta { get => _kPIDelta; set => SetProperty(ref _kPIDelta, value); }

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

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var stratum = parameters.GetValue<string>(NavParams.STRATUM);
            var sampleGroup = parameters.GetValue<string>(NavParams.SAMPLE_GROUP);
            var species = parameters.GetValue<string>(NavParams.SPECIES);
            var liveDead = parameters.GetValue<string>(NavParams.LIVE_DEAD);

            var tallyPopulation = TallyPopulationDataservice.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

            TallyPopulation = tallyPopulation;
            UnitCode = unit;
            CruiseMethod = tallyPopulation.Method;
        }

        public void SaveEdit()
        {
            if(_isSaved == true) { return; } // prevent double press

            var reason = EditReason;
            var treeCountDelta = TreeCountDelta;
            var kpiDelta = KPIDelta;
            var cruiser = DialogService.AskCruiserAsync();
            if (cruiser == null) { return; }

            var tallyLedger = new TallyLedger(UnitCode, TallyPopulation);
            tallyLedger.TreeCount = treeCountDelta;
            tallyLedger.KPI = kpiDelta;
            tallyLedger.Reason = reason;
            tallyLedger.Remarks = Remarks;
            tallyLedger.EntryType = TallyLedger.EntryTypeValues.TREECOUNT_EDIT;

            _isSaved = true;
            TallyDataservice.InsertTallyLedger(tallyLedger);

            NavigationService.GoBackAsync();
        }
    }
}