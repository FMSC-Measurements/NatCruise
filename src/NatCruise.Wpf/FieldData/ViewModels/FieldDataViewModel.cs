using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class FieldDataViewModel : ViewModelBase
    {
        private IEnumerable<CuttingUnit> _cuttingUnitOptions;
        private IEnumerable<Stratum> _stratumOptions;
        private IEnumerable<SampleGroup> _sampleGroupOptions;
        private CuttingUnit _selectedCuttingUnit;
        private Stratum _selectedStratum;
        private SampleGroup _selectedSampleGroup;

        public FieldDataViewModel(ICuttingUnitDataservice cuttingUnitDataservice,
                                  IStratumDataservice stratumDataservice,
                                  ISampleGroupDataservice sampleGroupDataservice,
                                  TreeListViewModel treeListViewModel,
                                  PlotListViewModel plotListViewModel,
                                  LogListViewModel logListViewModel)
        {
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            TreeListViewModel = treeListViewModel ?? throw new ArgumentNullException(nameof(treeListViewModel));
            PlotListViewModel = plotListViewModel ?? throw new ArgumentNullException(nameof(plotListViewModel));
            LogListViewModel = logListViewModel ?? throw new ArgumentNullException(nameof(logListViewModel));
        }

        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public IStratumDataservice StratumDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public TreeListViewModel TreeListViewModel { get; }
        public PlotListViewModel PlotListViewModel { get; }
        public LogListViewModel LogListViewModel { get; }
        public IEnumerable<CuttingUnit> CuttingUnitOptions
        {
            get => _cuttingUnitOptions;
            protected set => SetProperty(ref _cuttingUnitOptions, value);
        }

        public CuttingUnit SelectedCuttingUnit
        {
            get => _selectedCuttingUnit;
            set
            {
                SetProperty(ref _selectedCuttingUnit, value);
                var unitCode = value?.CuttingUnitCode;
                TreeListViewModel.CuttingUnitCode = unitCode;
                PlotListViewModel.CuttingUnitCode = unitCode;
                LogListViewModel.CuttingUnitCode = unitCode;
            }
        }

        public IEnumerable<Stratum> StratumOptions
        {
            get => _stratumOptions;
            protected set => SetProperty(ref _stratumOptions, value);
        }

        public Stratum SelectedStratum
        {
            get => _selectedStratum;
            set
            {
                SetProperty(ref _selectedStratum, value);
                var stCode = value?.StratumCode;
                TreeListViewModel.StratumCode = stCode;
                LogListViewModel.StratumCode = stCode;
            }
        }

        public IEnumerable<SampleGroup> SampleGroupOptions
        {
            get => _sampleGroupOptions;
            protected set => SetProperty(ref _sampleGroupOptions, value);
        }

        public SampleGroup SelectedSampleGroup
        {
            get => _selectedSampleGroup;
            set
            {
                SetProperty(ref _selectedSampleGroup, value);
                var sgCode = value?.SampleGroupCode;
                TreeListViewModel.SampleGroupCode = sgCode;
                LogListViewModel.SampleGroupCode = sgCode;
            }
        }

        public override void Load()
        {
            base.Load();

            CuttingUnitOptions = CuttingUnitDataservice.GetCuttingUnits();
            StratumOptions = StratumDataservice.GetStrata();
            SampleGroupOptions = SampleGroupDataservice.GetSampleGroups();

            RefreshFieldData();
        }

        protected void RefreshFieldData()
        {
            TreeListViewModel.Load();
            PlotListViewModel.Load();
            LogListViewModel.Load();
        }
    }
}
