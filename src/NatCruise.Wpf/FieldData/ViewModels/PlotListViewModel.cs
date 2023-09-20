using CommunityToolkit.Mvvm.Input;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using NatCruise.Async;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public partial class PlotListViewModel : ViewModelBase
    {
        private IEnumerable<Plot> _plots;
        private string _cuttingUnitCode;
        private Plot _selectedPlot;

        public PlotListViewModel(IPlotDataservice plotDataservice, ICuttingUnitDataservice cuttingUnitDataservice, PlotEditViewModel plotEditViewModel, INatCruiseDialogService dialogService)
        {
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            PlotEditViewModel = plotEditViewModel ?? throw new ArgumentNullException(nameof(plotEditViewModel));

            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public IPlotDataservice PlotDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }
        public PlotEditViewModel PlotEditViewModel { get; }
        public INatCruiseDialogService DialogService { get; }

        public event EventHandler PlotAdded;
        //public event EventHandler PlotRemoved;

        public Plot SelectedPlot
        {
            get => _selectedPlot;
            set
            {
                PlotEditViewModel.Plot = value;
                SetProperty(ref _selectedPlot, value);
            }
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

        public IEnumerable<Plot> Plots
        {
            get => _plots;
            set => SetProperty(ref _plots, value);
        }

        public override void Load()
        {
            base.Load();

            var plots = PlotDataservice.GetPlotsByUnitCode(CuttingUnitCode);
            Plots = plots;
        }

        [RelayCommand]
        public void AddPlot(int? plotNumber)
        {
            var unitCode = CuttingUnitCode;
            if (string.IsNullOrEmpty(unitCode))
            {
                DialogService.ShowMessageAsync("Please Select Cutting Unit First").FireAndForget();
                return;
            }

            var unitSummary = CuttingUnitDataservice.GetCuttingUnitStrataSummary(unitCode);
            if(!unitSummary.HasPlotStrata)
            {
                DialogService.ShowMessageAsync("Selected Unit Contains No Plot Strata").FireAndForget();
                return;
            }

            if (plotNumber.HasValue && !PlotDataservice.IsPlotNumberAvalible(unitCode, plotNumber.Value))
            {
                DialogService.ShowMessageAsync("Plot Number Already Exists").FireAndForget();
            }

            var plotID = (plotNumber.HasValue) ? PlotDataservice.AddNewPlot(CuttingUnitCode, plotNumber.Value)
                : PlotDataservice.AddNewPlot(CuttingUnitCode);
            Load();
            var plot = Plots.FirstOrDefault(x => x.PlotID == plotID);
            SelectedPlot = plot;
            PlotAdded?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        public void DeletePlot()
        {
            var plot = SelectedPlot;
            if (plot != null) { DeletePlot(plot); }
        }

        private void DeletePlot(Plot plot)
        {
            var unitCode = CuttingUnitCode;
            var plotNumber = plot.PlotNumber;

            PlotDataservice.DeletePlot(unitCode, plotNumber);
            Load();
        }
    }
}
