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
        }

        ICruiseDataservice CruiseDataservice { get; }

        public Sale Sale
        {
            get => _sale;
            set => SetProperty(ref _sale, value);
        } 


        protected override void Load()
        {
            Sale = CruiseDataservice.GetSale();
        }
    }
}
