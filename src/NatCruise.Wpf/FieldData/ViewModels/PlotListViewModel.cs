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

        public PlotListViewModel(IPlotDataservice plotDataservice)
        {
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
        }

        public IPlotDataservice PlotDataservice { get; }

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
