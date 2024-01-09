using NatCruise;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Common;
using System.Collections.Generic;

namespace FScruiser.XF.ViewModels
{
    public class SaleViewModel : ViewModelBase
    {
        private Sale _sale;

        protected ISaleDataservice Dataservice { get; set; }

        public SaleViewModel(IDataserviceProvider datastoreProvider)
        {
            Dataservice = datastoreProvider.GetDataservice<ISaleDataservice>();
        }

        public Sale Sale
        {
            get => _sale;
            set
            {
                OnSaleChanging(_sale);
                SetProperty(ref _sale, value);
                OnSaleChanged(_sale);
            }
        }

        private void OnSaleChanged(Sale sale)
        {
            if (sale != null)
            {
                sale.PropertyChanged += Sale_PropertyChanged;
            }
        }

        private void Sale_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            var sale = sender as Sale;
            if (propName == nameof(Sale.Remarks))
            {
                Dataservice.UpdateSale(sale);
            }
        }

        private void OnSaleChanging(Sale sale)
        {
            if (sale != null)
            {
                sale.PropertyChanged -= Sale_PropertyChanged;
            }
        }

        protected override void Load(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new System.ArgumentNullException(nameof(parameters)); }

            var cruiseID = parameters.GetValue<string>(NavParams.CruiseID);

            Sale = Dataservice.GetSaleByCruiseID(cruiseID);
        }
    }
}