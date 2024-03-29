﻿using NatCruise;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.Collections.Generic;

namespace FScruiser.XF.ViewModels
{
    // TODO delete unused class
    public class CruiseListViewModel : ViewModelBase
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

        public CruiseListViewModel(ISaleDataservice saleDataservice)
        {

            SaleDataservice = saleDataservice;
        }

        protected override void Load(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            var saleNumber = parameters.GetValue<string>(NavParams.SaleNumber);
            var saleDataservice = SaleDataservice;
            var sale = saleDataservice.GetSaleBySaleNumber(saleNumber);
            Sale = sale;

            var cruises = saleDataservice.GetCruisesBySaleNumber(saleNumber);
            Cruises = cruises;
        }
    }
}