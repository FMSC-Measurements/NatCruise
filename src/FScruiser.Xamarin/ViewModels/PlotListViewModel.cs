using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using Prism.Common;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotListViewModel : XamarinViewModelBase
    {
        private ICommand _addPlotCommand;
        private Command<Plot> _editPlotCommand;
        private ICommand _deletePlotCommand;

        public string UnitCode { get; set; }

        public bool HasFixCNTStrata { get; set; }

        public IEnumerable<Plot> Plots { get; protected set; }
        public IPageDialogService DialogService { get; }
        public ICuttingUnitDatastore Datastore { get; set; }
        protected ICruiseNavigationService NavigationService { get; }

        public ICommand AddPlotCommand => _addPlotCommand ?? (_addPlotCommand = new Command(AddPlot));

        public ICommand DeletePlotCommand => _deletePlotCommand ?? (_deletePlotCommand = new Command<Plot>(DeletePlot));

        public ICommand EditPlotCommand => _editPlotCommand ?? (_editPlotCommand = new Command<Plot>(ShowEditPlot));

        public PlotListViewModel(ICruiseNavigationService navigationService
            , IPageDialogService dialogService
            , IDataserviceProvider datastoreProvider)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            Datastore = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var unitCode = UnitCode = parameters.GetValue<string>(NavParams.UNIT);
            RefreshPlots();
        }

        private void RefreshPlots()
        {
            var unitCode = UnitCode;

            Plots = Datastore.GetPlotsByUnitCode(UnitCode).ToArray();
            RaisePropertyChanged(nameof(Plots));

            HasFixCNTStrata = Datastore.GetPlotStrataProxies(UnitCode)
                .Any(x => x.Method == CruiseDAL.Schema.CruiseMethods.FIXCNT);
        }

        public void AddPlot(object obj)
        {
            var plotID = Datastore.AddNewPlot(UnitCode);
            //NavigationService.NavigateAsync($"PlotEdit?{NavParams.PlotID}={plotID}");
            NavigationService.ShowPlotEdit(plotID);
        }

        private void DeletePlot(Plot plot)
        {
            var unitCode = UnitCode;
            var plotNumber = plot.PlotNumber;

            Datastore.DeletePlot(unitCode, plotNumber);

            RefreshPlots();
        }

        public void ShowEditPlot(Plot plot)
        {
            //NavigationService.NavigateAsync($"PlotEdit?{NavParams.PlotID}={plot.PlotID}");
            NavigationService.ShowPlotEdit(plot.PlotID);
        }

        public async void ShowTallyPlot(Plot plot)
        {
            var fixCNTstrata = Datastore.GetPlotStrataProxies(UnitCode).Where(x => x.Method == CruiseDAL.Schema.CruiseMethods.FIXCNT).ToArray();

            if (fixCNTstrata.Any()
                && await DialogService.DisplayAlertAsync("Show FixCNT Tally Page?", "", "FixCNT", "Standard"))
            {
                string stratum = null;
                if (fixCNTstrata.Count() == 1)
                {
                    stratum = fixCNTstrata.Single().StratumCode;
                }
                else
                {
                    stratum = await DialogService.DisplayActionSheetAsync("Select Stratum", "Cancel", "", fixCNTstrata.Select(x => x.StratumCode).ToArray());
                }

                if (stratum == null || stratum == "Cancel") { return; }

                //await NavigationService.NavigateAsync($"FixCNTTally?{NavParams.UNIT}={UnitCode}&{NavParams.STRATUM}={stratum}&{NavParams.PLOT_NUMBER}={plot.PlotNumber}");

                await NavigationService.ShowFixCNT(UnitCode, plot.PlotNumber, stratum);
            }
            else
            {
                //await NavigationService.NavigateAsync($"PlotTally?{NavParams.PlotID}={plot.PlotID}");

                await NavigationService.ShowPlotTally(plot.PlotID);
            }
        }
    }
}