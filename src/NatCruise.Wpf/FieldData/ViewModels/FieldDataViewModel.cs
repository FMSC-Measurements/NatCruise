using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
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
        private IEnumerable<Plot> _plotOptions;
        private Plot _selectedPlot;

        public FieldDataViewModel(ICuttingUnitDataservice cuttingUnitDataservice,
                                  IStratumDataservice stratumDataservice,
                                  ISampleGroupDataservice sampleGroupDataservice,
                                  IPlotDataservice plotDataservice,
                                  TreeListViewModel treeListViewModel,
                                  PlotListViewModel plotListViewModel,
                                  LogListViewModel logListViewModel,
                                  TallyPopulationListViewModel tallyPopulationListViewModel)
        {
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            TreeListViewModel = treeListViewModel ?? throw new ArgumentNullException(nameof(treeListViewModel));
            PlotListViewModel = plotListViewModel ?? throw new ArgumentNullException(nameof(plotListViewModel));
            LogListViewModel = logListViewModel ?? throw new ArgumentNullException(nameof(logListViewModel));
            TallyPopulationListViewModel = tallyPopulationListViewModel ?? throw new ArgumentNullException(nameof(tallyPopulationListViewModel));
        }

        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public IStratumDataservice StratumDataservice { get; }
        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public IPlotDataservice PlotDataservice { get; }
        public TreeListViewModel TreeListViewModel { get; }
        public PlotListViewModel PlotListViewModel { get; }
        public LogListViewModel LogListViewModel { get; }
        public TallyPopulationListViewModel TallyPopulationListViewModel { get; }
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
                if (value == _selectedCuttingUnit) { return; }
                SetProperty(ref _selectedCuttingUnit, value);
                var unitCode = value?.CuttingUnitCode;
                TreeListViewModel.CuttingUnitCode = unitCode;
                PlotListViewModel.CuttingUnitCode = unitCode;
                LogListViewModel.CuttingUnitCode = unitCode;
                TallyPopulationListViewModel.CuttingUnitCode = unitCode;
                RefreshPlotOptions();
                RefreshStratumOptions();
            }
        }

        public IEnumerable<Plot> PlotOptions
        {
            get => _plotOptions;
            set => SetProperty(ref _plotOptions, value);
        }

        public Plot SelectedPlot
        {
            get => _selectedPlot;
            set
            {
                if (value == _selectedPlot) { return; }
                SetProperty(ref _selectedPlot, value);
                TreeListViewModel.PlotNumber = value?.PlotNumber;
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
                if (value == _selectedStratum) { return; }
                SetProperty(ref _selectedStratum, value);
                var stCode = value?.StratumCode;
                TreeListViewModel.StratumCode = stCode;
                LogListViewModel.StratumCode = stCode;
                TallyPopulationListViewModel.StratumCode = stCode;
                RefreshSampleGroupOptions();
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
                if (value == _selectedSampleGroup) { return; }
                SetProperty(ref _selectedSampleGroup, value);
                var sgCode = value?.SampleGroupCode;
                TreeListViewModel.SampleGroupCode = sgCode;
                LogListViewModel.SampleGroupCode = sgCode;
                TallyPopulationListViewModel.SampleGroupCode = sgCode;
            }
        }

        public override void Load()
        {
            base.Load();

            RefreshCuttingUnitOptions();
            RefreshPlotOptions();
            RefreshStratumOptions();
            RefreshSampleGroupOptions();

            TreeListViewModel.Load();
            PlotListViewModel.Load();
            LogListViewModel.Load();
            TallyPopulationListViewModel.Load();
        }

        public void RefreshCuttingUnitOptions()
        {
            CuttingUnitOptions = CuttingUnitDataservice.GetCuttingUnits()
                .OrderBy(x => x.CuttingUnitCode)
                .ToArray();
        }

        public void RefreshPlotOptions()
        {
            var selectedUnitCode = SelectedCuttingUnit?.CuttingUnitCode;
            if (selectedUnitCode != null)
            {
                PlotOptions = PlotDataservice.GetPlotsByUnitCode(selectedUnitCode)
                    .OrderBy(X => X.PlotNumber)
                    .ToArray();
            }
            else
            { PlotOptions = new Plot[0]; }
        }

        public void RefreshStratumOptions()
        {
            var cuttingUnitCode = SelectedCuttingUnit?.CuttingUnitCode;
            if(cuttingUnitCode != null)
            {
                StratumOptions = StratumDataservice.GetStrata(cuttingUnitCode);
            }
            else
            {
                StratumOptions = StratumDataservice.GetStrata();
            }
        }

        public void RefreshSampleGroupOptions()
        {
            var stratumCode = SelectedStratum?.StratumCode;
            if(stratumCode != null)
            {
                SampleGroupOptions = SampleGroupDataservice.GetSampleGroups(stratumCode);
            }
            else
            {
                SampleGroupOptions = new SampleGroup[0];
            }
        }
    }
}
