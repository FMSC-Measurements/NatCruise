using FScruiser.XF.Constants;
using NatCruise.Data;
using NatCruise.Data.Abstractions;
using NatCruise.Models;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.ViewModels
{
    public class CruiseListViewModel : XamarinViewModelBase
    {
        private Sale _sale;
        private IEnumerable<Cruise> _cruises;

        protected ISaleDataservice SaleDataservice { get; }

        public Sale Sale
        {
            get => _sale;
            protected set => SetProperty(ref _sale, value);
        }

        public IEnumerable<Cruise> Cruises
        {
            get => _cruises;
            set => SetProperty(ref _cruises, value);
        }

        public CruiseListViewModel(IDataserviceProvider dataserviceProvider)
        {
            if (dataserviceProvider is null) { throw new ArgumentNullException(nameof(dataserviceProvider)); }

            SaleDataservice = dataserviceProvider.GetDataservice<ISaleDataservice>();
        }

        protected override void Load(IParameters parameters)
        {
            var saleID = parameters.GetValue<string>(NavParams.SaleID);
            var saleDataservice = SaleDataservice;
            var sale = saleDataservice.GetSale(saleID);
            Sale = sale;

            var cruises = saleDataservice.GetCruises(saleID);
            Cruises = cruises;

        }
    }
}
