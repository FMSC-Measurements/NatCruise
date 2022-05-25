using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class PlotListViewModel : ViewModelBase
    {
        private IEnumerable<Plot> _plots;
        private string _cuttingUnitCode;
        private Plot _selectedPlot;

        public PlotListViewModel(IPlotDataservice plotDataservice, PlotEditViewModel plotEditViewModel)
        {
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            PlotEditViewModel = plotEditViewModel ?? throw new ArgumentNullException(nameof(plotEditViewModel));
        }

        public IPlotDataservice PlotDataservice { get; }
        public PlotEditViewModel PlotEditViewModel { get; }

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
    }
}
