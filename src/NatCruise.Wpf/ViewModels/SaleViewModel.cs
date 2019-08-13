using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.ViewModels
{
    public class SaleViewModel : ViewModelBase
    {
        private Sale _sale;

        public SaleViewModel(IDataserviceProvider dataserviceProvider)
        {
            CruiseDataservice = dataserviceProvider.GetDataservice<ICruiseDataservice>();
            SetupinfoDataservice = dataserviceProvider.GetDataservice<ISetupInfoDataservice>();
        }

        ICruiseDataservice CruiseDataservice { get; }
        public ISetupInfoDataservice SetupinfoDataservice { get; }
        public Sale Sale
        {
            get => _sale;
            set => SetProperty(ref _sale, value);
        }

        public IEnumerable<Purpose> PurposeOptions => SetupinfoDataservice.GetPurposes();

        public IEnumerable<Region> RegionOptions => SetupinfoDataservice.GetRegions();

        public IEnumerable<Forest> ForestOptions
        {
            get => SetupinfoDataservice.GetForests(Sale?.Region ?? "");
        }


        protected override void Load()
        {
            Sale = CruiseDataservice.GetSale();
            RaisePropertyChanged(nameof(ForestOptions));
        }
    }
}
